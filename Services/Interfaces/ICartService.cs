using ShoppingOnline.DTOs.Cart;

namespace ShoppingOnline.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCart(int userId, AddToCartDTO dto);

        Task UpdateCart(int userId, UpdateCartDTO dto);

        Task RemoveItem(int userId, int productId);

        Task<List<CartItemResponseDTO>> GetCart(int userId);
    }
}