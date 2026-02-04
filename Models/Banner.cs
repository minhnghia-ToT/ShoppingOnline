namespace ShoppingOnline.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
