using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EC_V2.Dtos;
using EC_V2.Services.Implementations;
using EC_V2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenBlacklistService _blacklist;
        public AuthController(IAuthService auth,ILogger<AuthController> logger, ITokenBlacklistService blacklist)
        {
            _auth = auth;
            _logger = logger;
            _blacklist = blacklist;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            _logger.LogInformation("Registering user with phone: {Phone}", dto.Phone);
            var result = await _auth.Register(dto);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Data);
        }

        [HttpPost("verify-otp")]
        public async Task <IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            _logger.LogInformation("Verifying OTP for phone: {Phone} and purpose: {Purpose}", dto.Phone, dto.Purpose);
            var result = await _auth.VerifyOtp(dto);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Data);
        }



         [HttpPost("login")]
         public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Logging in user with phone: {Phone}", dto.Phone);
            var result = await _auth.Login(dto);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Data);
        }


        [HttpPost("verify-login")]
        public async Task<IActionResult> VerifyLogin(VerifyLoginDto dto)
        {
            _logger.LogInformation("Verifying login for phone: {Phone}", dto.Phone);
            var result = await _auth.VerifyLogin(dto);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Data);
         }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _auth.RefreshToken(refreshToken);
            if (!result.Success) return BadRequest(result.Error);
            return Ok(result.Data);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (jti != null)
                _blacklist.BlacklistToken(jti, DateTime.UtcNow.AddMinutes(15));

            var result = await _auth.Logout(userId);
            if (!result.Success) return BadRequest(result.Error);
            return Ok("Logged out successfully");
        }


    }
}
