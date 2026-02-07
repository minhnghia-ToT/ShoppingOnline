using ShoppingOnline.DTOs;

namespace ShoppingOnline.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
       
        Task<string> RegisterAsync(RegisterRequestDto request);
        Task<string> RegisterAdminAsync(RegisterAdminRequestDto request);

        // ===== FORGOT PASSWORD =====
        Task SendForgotPasswordOtpAsync(ForgotPasswordRequestDto request);
        Task VerifyOtpAsync(VerifyOtpRequestDto request);
        Task ResetPasswordAsync(string email, string code, string newPassword);
        
    }
}
