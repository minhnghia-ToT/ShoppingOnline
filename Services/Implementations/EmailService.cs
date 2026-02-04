using Microsoft.Extensions.Configuration;
using ShoppingOnline.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ShoppingOnline.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string email, string otp)
        {
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var password = _config["EmailSettings:Password"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"] ?? "587");

            if (string.IsNullOrEmpty(senderEmail))
                throw new Exception("SenderEmail missing in appsettings.json");

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Password Reset OTP",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = false
            };

            mail.To.Add(email);

            using var smtp = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }
    }
}
