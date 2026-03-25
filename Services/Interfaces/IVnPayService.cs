using ShoppingOnline.Models;

namespace OnlineShopping.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, Order order);
        bool ValidateSignature(IQueryCollection query);
    }
}
