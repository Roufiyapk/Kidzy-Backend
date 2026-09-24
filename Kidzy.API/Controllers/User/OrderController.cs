using System.Net;
using System.Security.Claims;
using FluentValidation;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.User;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController
    : ControllerBase
{
    private readonly IOrderService _orderService;

    private readonly IValidator<CreateOrderDto>
        _createOrderValidator;

    private readonly IValidator<BuyNowOrderDto>
        _buyNowOrderValidator;

    public OrderController(
        IOrderService orderService,
        IValidator<CreateOrderDto> createOrderValidator,
        IValidator<BuyNowOrderDto> buyNowOrderValidator)
    {
        _orderService =
            orderService;

        _createOrderValidator =
            createOrderValidator;

        _buyNowOrderValidator =
            buyNowOrderValidator;
    }


    // CART
    
    [HttpPost("cart")]
    public async Task<IActionResult>
        CreateFromCart(
            [FromBody]
            CreateOrderDto dto)
    {
        var validationResult =
            await _createOrderValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<CheckoutResponseDto>.Fail(
                    validationResult.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList(),

                    "Validation failed."));
        }


        try
        {
            var userId =
                GetUserId();

            var result =
                await _orderService
                    .CreateOrderFromCartAsync(
                        userId,
                        dto);


            // Razorpay first call

            if (result.PaymentRequired)
            {
                return Ok(
                    ApiResponse<CheckoutResponseDto>
                        .Success(
                            result,
                            "Razorpay order created successfully."));
            }


            // Actual Order created

            return StatusCode(
                (int)HttpStatusCode.Created,

                ApiResponse<CheckoutResponseDto>
                    .Success(
                        result,
                        "Order placed successfully.",
                        HttpStatusCode.Created));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<CheckoutResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Unauthorized.",

                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<CheckoutResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Failed to process order."));
        }
    }


    // BUY NOW
    

    [HttpPost("buy-now")]
    public async Task<IActionResult>
        BuyNow(
            [FromBody]
            BuyNowOrderDto dto)
    {
        var validationResult =
            await _buyNowOrderValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                ApiResponse<CheckoutResponseDto>.Fail(
                    validationResult.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList(),

                    "Validation failed."));
        }


        try
        {
            var userId =
                GetUserId();

            var result =
                await _orderService
                    .CreateBuyNowOrderAsync(
                        userId,
                        dto);


            // Razorpay first call

            if (result.PaymentRequired)
            {
                return Ok(
                    ApiResponse<CheckoutResponseDto>
                        .Success(
                            result,
                            "Razorpay order created successfully."));
            }


            // Actual Order created

            return StatusCode(
                (int)HttpStatusCode.Created,

                ApiResponse<CheckoutResponseDto>
                    .Success(
                        result,
                        "Order placed successfully.",
                        HttpStatusCode.Created));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<CheckoutResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Unauthorized.",

                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<CheckoutResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Failed to process order."));
        }
    }


    // GET USER ORDERS

    [HttpGet]
    public async Task<IActionResult>
        GetOrders()
    {
        try
        {
            var userId =
                GetUserId();

            var orders =
                await _orderService
                    .GetUserOrdersAsync(
                        userId);

            return Ok(
                ApiResponse<List<OrderResponseDto>>
                    .Success(
                        orders,
                        "Orders retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<List<OrderResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Unauthorized.",

                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<OrderResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Failed to retrieve orders."));
        }
    }


    // GET SINGLE ORDER

    [HttpGet("{id:int}")]
    public async Task<IActionResult>
        GetOrderById(int id)
    {
        try
        {
            var userId =
                GetUserId();

            var order =
                await _orderService
                    .GetOrderByIdAsync(
                        userId,
                        id);

            if (order == null)
            {
                return NotFound(
                    ApiResponse<OrderResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },

                            "Order not found.",

                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<OrderResponseDto>
                    .Success(
                        order,
                        "Order retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<OrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Unauthorized.",

                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<OrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Failed to retrieve order."));
        }
    }


    // CANCEL

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult>
        CancelOrder(int id)
    {
        try
        {
            var userId =
                GetUserId();

            var order =
                await _orderService
                    .CancelOrderAsync(
                        userId,
                        id);

            if (order == null)
            {
                return NotFound(
                    ApiResponse<OrderResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },

                            "Order not found.",

                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<OrderResponseDto>
                    .Success(
                        order,
                        "Order cancelled successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                ApiResponse<OrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Unauthorized.",

                        HttpStatusCode.Unauthorized));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<OrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },

                        "Failed to cancel order."));
        }
    }


    // =====================================================
    // JWT USER ID
    // =====================================================

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