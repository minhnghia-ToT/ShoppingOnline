namespace ShoppingOnline.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiredAt { get; set; }
    }
}
