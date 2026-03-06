namespace ShoppingOnline.Services.Interfaces
{
    public interface IEmailService
    {

        Task SendOtpAsync(string email, string otp);
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
