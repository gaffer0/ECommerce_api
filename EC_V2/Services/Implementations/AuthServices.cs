using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EC_V2.Data;
using EC_V2.Dtos;
using EC_V2.Models;
using EC_V2.Services.Interfaces;
using EC_V2.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using PhoneNumbers;

namespace EC_V2.Services.Implementations
{
    public class AuthServices: IAuthService
    {
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOptions<JWTSettings> _jwtSettings;
        public AuthServices(IPasswordHasher<AppUser> passwordHasher, AppDbContext appDbContext, UserManager<AppUser> userManager,IOptions<JWTSettings> options)
        {
            _passwordHasher = passwordHasher;
            _db = appDbContext;
            _userManager = userManager;
            _jwtSettings = options;
        }

        public async Task<ServiceResult<OtpResponseDto>> Register(RegisterDto dto)
        {
            // 1. Validate phone
            if (!ValidateNumber(dto.Phone))
                return new ServiceResult<OtpResponseDto> { Success = false, Error = "Invalid phone number" };

            // 2. Check if user already exists
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Phone);
            if (existingUser != null)
                return new ServiceResult<OtpResponseDto> { Success = false, Error = "Phone already registered" };

            // 3. Create user
            var user = new AppUser
            {
                PhoneNumber = dto.Phone,
                UserName = dto.Phone,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return new ServiceResult<OtpResponseDto> { Success = false, Error = "Failed to create user" };
            // 4. Generate OTP
            var otp = GenerateOtp();
            // 5. Hash OTP
            var otpHash = HashOtp(otp);
            // 6. Save OTP
            await SaveOtp(user.Id, otpHash, "Registration");
            // 7. Return OTP (in real app, send via SMS)
            return new ServiceResult<OtpResponseDto>
            {
                Success = true,
                Data = new OtpResponseDto
                {
                    Message = "OTP sent to your phone",
                    OtpCode = otp // In production, do NOT return OTP in response
                }
            };

        }



        public async Task<ServiceResult<OtpResponseDto>> Login(LoginDto dto)
        {
            if (!ValidateNumber(dto.Phone))
                return new ServiceResult<OtpResponseDto> { Success = false, Error = "Invalid phone number" };

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Phone);
            if (user == null)
                return new ServiceResult<OtpResponseDto> { Success = false, Error = "User not found" };

            var otp = GenerateOtp();
            var otpHash = HashOtp(otp);
            await SaveOtp(user.Id, otpHash, "Login");

            return new ServiceResult<OtpResponseDto>
            {
                Success = true,
                Data = new OtpResponseDto
                {
                    Message = "OTP sent to your phone",
                    OtpCode = otp
                }
            };
        }

        public async Task<ServiceResult<bool>> VerifyOtp(VerifyOtpDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Phone);
            if (user == null)
                return new ServiceResult<bool> { Success = false, Error = "User not found" };

            var otpEntry = await _db.otpCodes
                .Where(o => o.UserId == user.Id && o.Purpose == dto.Purpose && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpEntry == null)
                return new ServiceResult<bool> { Success = false, Error = "OTP not found" };

            if (otpEntry.ExpiresAt < DateTime.UtcNow)
                return new ServiceResult<bool> { Success = false, Error = "OTP expired" };

            if (!VerifyOtp(otpEntry.CodeHash, dto.Otp))
                return new ServiceResult<bool> { Success = false, Error = "Invalid OTP" };

            otpEntry.IsUsed = true;
            await _db.SaveChangesAsync();

            return new ServiceResult<bool> { Success = true, Data = true };
        }

        public async Task<ServiceResult<AuthResponseDto>> VerifyLogin(VerifyLoginDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.Phone);
            if (user == null)
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "User not found" };

            var otpEntry = await _db.otpCodes
                .Where(o => o.UserId == user.Id && o.Purpose == "Login" && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpEntry == null)
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "OTP not found" };

            if (otpEntry.ExpiresAt < DateTime.UtcNow)
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "OTP expired" };

            if (!VerifyOtp(otpEntry.CodeHash, dto.Otp))
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "Invalid OTP" };

            otpEntry.IsUsed = true;
            await _db.SaveChangesAsync();

            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshToken(user);

            return new ServiceResult<AuthResponseDto>
            {
                Success = true,
                Data = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15)
                }
            };
        }

        public async Task<ServiceResult<AuthResponseDto>> RefreshToken(string refreshToken)
        {
            var tokenHash = HashOtp(refreshToken);
            var token = await _db.refreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

            if (token == null || token.IsUsed || token.RevokedAt != null)
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "Invalid refresh token" };

            if (token.ExpiresAt < DateTime.UtcNow)
                return new ServiceResult<AuthResponseDto> { Success = false, Error = "Refresh token expired" };

            // Rotate — invalidate old token
            token.IsUsed = true;
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Generate new tokens
            var newAccessToken = GenerateAccessToken(token.User);
            var newRefreshToken = await GenerateRefreshToken(token.User);

            return new ServiceResult<AuthResponseDto>
            {
                Success = true,
                Data = new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15)
                }
            };
        }

        public async Task<ServiceResult<bool>> Logout(string userId)
        {
            var tokens = await _db.refreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null && !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.IsUsed = true;
            }

            await _db.SaveChangesAsync();
            return new ServiceResult<bool> { Success = true, Data = true };
        }














        private bool ValidateNumber(string phone)
        {
            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneUtil.Parse(phone, null);

                var regionCode = phoneUtil.GetRegionCodeForNumber(parsedNumber);
                var allowedRegions = new[] { "EG", "SA" }; // Egypt and Saudi

                return phoneUtil.IsValidNumber(parsedNumber) &&
                       allowedRegions.Contains(regionCode);
            }
            catch
            {
                return false;
            }
        }

        private string GenerateOtp()
        {          
            Random random = new Random();
            int otp = random.Next(1000, 9999);
            return otp.ToString();
        }
        private string HashOtp(string otp)
        {
            return _passwordHasher.HashPassword(null!, otp);
        }
        private bool VerifyOtp(string hashedOtp, string providedOtp)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hashedOtp, providedOtp);
            return result == PasswordVerificationResult.Success;
        }
        private async Task SaveOtp(string userId, string otpHash, string purpose)
        {
            var otpEntry = new OtpCode
            {
                UserId = userId,
                CodeHash = otpHash,
                Purpose = purpose,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };
            _db.otpCodes.Add(otpEntry);
            await _db.SaveChangesAsync();

        }
        private async Task<string> GenerateRefreshToken(AppUser user)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var tokenHash = HashOtp(token);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpirationDays),
                IsUsed = false
            };

            _db.refreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return token;
        }
        private string GenerateAccessToken(AppUser user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
