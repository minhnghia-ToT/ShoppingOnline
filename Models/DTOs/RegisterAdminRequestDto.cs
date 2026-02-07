namespace ShoppingOnline.DTOs
{
    public class RegisterAdminRequestDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}