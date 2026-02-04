using Microsoft.EntityFrameworkCore;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs;
using ShoppingOnline.Helpers;
using ShoppingOnline.Models;
using ShoppingOnline.Services.Interfaces;


namespace ShoppingOnline.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        public AuthService(ApplicationDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;

        }

        // ===================== LOGIN =====================
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email == request.Email &&
                    u.IsActive);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var roles = user.UserRoles
                .Select(ur => ur.Role.Name)
                .ToList();

            var token = JwtHelper.GenerateToken(user, roles, _config);

            return new LoginResponseDto
            {
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddHours(2)
            };
        }

        // ===================== REGISTER =====================
        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (exists)
                return "Email already exists.";

            var userRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "User");

            if (userRole == null)
                return "Default role not found.";

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = userRole.Id
            });

            await _context.SaveChangesAsync();

            return "Register successfully.";
        }
        // ===================== FORGOT PASSWORD =====================
        public async Task SendForgotPasswordOtpAsync(ForgotPasswordRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null) throw new Exception("User not found");

            var code = new Random().Next(1000, 9999).ToString();

            var otp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = code,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.PasswordResetOtps.Add(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpAsync(user.Email, code);
        }

        public async Task<string> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            var otp = await _context.PasswordResetOtps
                .Include(o => o.User)
                .Where(o =>
                    o.User.Email == request.Email &&
                    !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return "OTP not found.";

            if (otp.ExpiredAt < DateTime.UtcNow)
                return "OTP has expired.";

            if (otp.Code != request.OtpCode)
                return "Invalid OTP. Please try again.";

            return "OTP is valid.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var otp = await _context.PasswordResetOtps
                .Include(o => o.User)
                .Where(o =>
                    o.User.Email == request.Email &&
                    !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return "OTP not found.";

            if (otp.ExpiredAt < DateTime.UtcNow)
                return "OTP has expired.";

            if (otp.Code != request.OtpCode)
                return "Invalid OTP.";

            otp.IsUsed = true;
            otp.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _context.SaveChangesAsync();

            return "Password has been reset successfully.";
        }
    }
}
