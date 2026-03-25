using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models.DTOs;
using OnlineShopping.Services.Interfaces;
using ShoppingOnline.Data;
using ShoppingOnline.Models;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IVnPayService _vnPayService;

    public PaymentService(ApplicationDbContext context, IVnPayService vnPayService)
    {
        _context = context;
        _vnPayService = vnPayService;
    }

    // =============================
    // CREATE PAYMENT
    // =============================
    public async Task<PaymentResponseDTO> CreateVnPayPayment(HttpContext context, CreatePaymentDTO dto)
    {
        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

        if (order == null)
            throw new Exception("Order not found");

        if (order.Payment.Method != "VNPAY")
            throw new Exception("Payment method must be VNPAY");

        if (order.Status != "Pending")
            throw new Exception("Order already processed");

        var paymentUrl = _vnPayService.CreatePaymentUrl(context, order);

        return new PaymentResponseDTO
        {
            PaymentUrl = paymentUrl
        };
    }

    // =============================
    // HANDLE IPN
    // =============================
    public async Task HandleVnPayIPN(IQueryCollection query)
    {
        if (!_vnPayService.ValidateSignature(query))
            throw new Exception("Invalid signature");

        var txnRef = query["vnp_TxnRef"];
        var responseCode = query["vnp_ResponseCode"];

        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id.ToString() == txnRef);

        if (order == null)
            throw new Exception("Order not found");

        if (responseCode == "00")
        {
            order.Status = "Paid";
            order.Payment.Status = "Success";
        }
        else
        {
            order.Status = "Failed";
            order.Payment.Status = "Failed";
        }

        _context.OrderHistories.Add(new OrderHistory
        {
            OrderId = order.Id,
            Status = order.Status
        });

        await _context.SaveChangesAsync();
    }
}