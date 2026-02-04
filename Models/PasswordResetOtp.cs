using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.Models
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        /*[Required]
        public string OtpCode { get; set; }*/
        public string Code { get; set; } = null!; // 4 digits
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
