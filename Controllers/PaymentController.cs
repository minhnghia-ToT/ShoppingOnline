using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Models.DTOs;
using OnlineShopping.Services.Interfaces;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // =============================
    // CREATE PAYMENT
    // =============================
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDTO dto)
    {
        var result = await _paymentService.CreateVnPayPayment(HttpContext, dto);
        return Ok(result);
    }

    // =============================
    // IPN
    // =============================
    [HttpGet("ipn")]
    public async Task<IActionResult> Ipn()
    {
        await _paymentService.HandleVnPayIPN(Request.Query);
        return Ok("OK");
    }

    // =============================
    // RETURN
    // =============================
    [HttpGet("return")]
    public IActionResult Return()
    {
        var responseCode = Request.Query["vnp_ResponseCode"];

        if (responseCode == "00")
            return Redirect("http://localhost:3000/payment-success");

        return Redirect("http://localhost:3000/payment-failed");
    }
}