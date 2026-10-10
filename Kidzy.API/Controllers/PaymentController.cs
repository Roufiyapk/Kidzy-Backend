using System.Net;
using System.Security.Claims;

using Kidzy.API.Contracts;

using Kidzy.Application.DTOs.Payments;
using Kidzy.Application.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentController
    : ControllerBase
{
    private readonly IPaymentService
        _paymentService;


    public PaymentController(
        IPaymentService paymentService)
    {
        _paymentService =
            paymentService;
    }


    // POST /api/payments/create-order

    [HttpPost("create-order")]
    public async Task<IActionResult>
        CreateOrder(
            [FromBody]
            CreatePaymentOrderDto dto)
    {
        try
        {
            var userId =
                GetUserId();


            var result =
                await _paymentService
                    .CreateOrderAsync(
                        userId,
                        dto);


            return Ok(
                ApiResponse<PaymentResponseDto>
                    .Success(
                        result,
                        "Payment order created successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<PaymentResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to create payment order."));
        }
    }


    // POST /api/payments/verify

    [HttpPost("verify")]
    public async Task<IActionResult>
        VerifyPayment(
            [FromBody]
            VerifyPaymentDto dto)
    {
        try
        {
            var userId =
                GetUserId();


            var result =
                await _paymentService
                    .VerifyPaymentAsync(
                        userId,
                        dto);


            return Ok(
                ApiResponse<PaymentResponseDto>
                    .Success(
                        result,
                        "Payment verified successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<PaymentResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Unauthorized."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<PaymentResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Payment verification failed."));
        }
    }


    // GET /api/payments/{id}

    [HttpGet("{id:int}")]
    public async Task<IActionResult>
        GetPayment(
            int id)
    {
        try
        {
            var userId =
                GetUserId();


            var result =
                await _paymentService
                    .GetPaymentAsync(
                        userId,
                        id);


            if (result == null)
            {
                return NotFound(
                    ApiResponse<PaymentResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Payment not found."
                            },
                            "Payment not found.",
                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<PaymentResponseDto>
                    .Success(
                        result,
                        "Payment retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<PaymentResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve payment."));
        }
    }


    // USER ID

    private int GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (string.IsNullOrWhiteSpace(
                userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }


        if (!int.TryParse(
                userId,
                out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user ID in token.");
        }


        return parsedUserId;
    }
}