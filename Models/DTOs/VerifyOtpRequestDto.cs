namespace ShoppingOnline.DTOs
{
    public class VerifyOtpRequestDto
    {
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
    }
}
