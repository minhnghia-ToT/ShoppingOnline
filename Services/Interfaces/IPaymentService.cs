using OnlineShopping.Models.DTOs;

namespace OnlineShopping.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreateVnPayPayment(HttpContext context, CreatePaymentDTO dto);
        Task HandleVnPayIPN(IQueryCollection query);
    }
}
