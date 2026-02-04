using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.DTOs;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            var message = await _authService.RegisterAsync(request);

            if (message != "Register successfully.")
                return BadRequest(new { message });

            return Ok(new { message });
        }
        // ===================== FORGOTPASSWORD =====================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            await _authService.SendForgotPasswordOtpAsync(request);
            return Ok("OTP sent to email");
        }

        /* [HttpPost("verify-otp")]
         public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto request)
         {
             var result = await _authService.VerifyOtpAsync(request);
             return Ok(result);
         }*/

        /* [HttpPost("reset-password")]
         public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
         {
             var result = await _authService.ResetPasswordAsync(request);
             return Ok(result);
         }*/
    }
}
