namespace ShoppingOnline.DTOs.Categories
{
    public class CategoryAdminListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}