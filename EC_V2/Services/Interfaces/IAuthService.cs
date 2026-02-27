using EC_V2.Dtos;

namespace EC_V2.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<OtpResponseDto>> Register(RegisterDto dto);
        Task<ServiceResult<OtpResponseDto>> Login(LoginDto dto);
        Task<ServiceResult<bool>> VerifyOtp(VerifyOtpDto dto);
        Task<ServiceResult<AuthResponseDto>> VerifyLogin(VerifyLoginDto dto);
        Task<ServiceResult<AuthResponseDto>> RefreshToken(string refreshToken);
        Task<ServiceResult<bool>> Logout(string userId);
    }
}
