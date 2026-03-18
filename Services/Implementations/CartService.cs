using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using ShoppingOnline.Data;
using ShoppingOnline.DTOs.Cart;
using ShoppingOnline.Services.Interfaces;

namespace ShoppingOnline.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===============================
        // ADD TO CART
        // ===============================
        public async Task AddToCart(int userId, AddToCartDTO dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            if (dto.Quantity > product.StockQuantity)
                throw new Exception($"Only {product.StockQuantity} items available in stock");

            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

            var item = cart.CartItems
                .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (item == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Price = product.DiscountPrice ?? product.Price
                });
            }
            else
            {
                if (item.Quantity + dto.Quantity > product.StockQuantity)
                    throw new Exception($"Only {product.StockQuantity} items available in stock");

                item.Quantity += dto.Quantity;
                item.Price = product.DiscountPrice ?? product.Price;
            }

            await _context.SaveChangesAsync();
        }

        // ===============================
        // UPDATE CART
        // ===============================
        public async Task UpdateCart(int userId, UpdateCartDTO dto)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (item == null)
                throw new Exception("Item not found in cart");

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

            if (product == null)
                throw new Exception("Product not found");

            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                if (dto.Quantity > product.StockQuantity)
                    throw new Exception($"Only {product.StockQuantity} items available in stock");

                item.Quantity = dto.Quantity;

                // update price nếu product thay đổi
                item.Price = product.DiscountPrice ?? product.Price;
            }

            await _context.SaveChangesAsync();
        }

        // ===============================
        // REMOVE ITEM
        // ===============================
        public async Task RemoveItem(int userId, int productId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.CartItems
                .FirstOrDefault(x => x.ProductId == productId);

            if (item == null)
                throw new Exception("Item not found");

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }

        // ===============================
        // GET CART
        // ===============================
        public async Task<List<CartItemResponseDTO>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                return new List<CartItemResponseDTO>();

            return cart.CartItems.Select(x => new CartItemResponseDTO
            {
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Image = x.Product.Images
                .FirstOrDefault(i => i.IsMain)?.ImageUrl,

                Price = x.Price,
                Quantity = x.Quantity

                // ❌ KHÔNG gán Total nữa
            }).ToList();
        }
    }
}