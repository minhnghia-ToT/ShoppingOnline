using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.DTOs
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
