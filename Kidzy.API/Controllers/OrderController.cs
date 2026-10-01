using System.Net;
using System.Security.Claims;

using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService
        _orderService;

    public OrderController(
        IOrderService orderService)
    {
        _orderService =
            orderService;
    }


    // USER
    // CREATE ORDER

    [HttpPost]
    [Authorize]
    public async Task<IActionResult>
        CreateOrder(
            [FromBody]
            CreateOrderDto dto)
    {
        try
        {
            var userId =
                GetUserId();

            var result =
                await _orderService
                    .CreateOrderFromCartAsync(
                        userId,
                        dto);

            return Ok(
                ApiResponse<CheckoutResponseDto>
                    .Success(
                        result,
                        "Order created successfully."));
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
                        "Failed to create order."));
        }
    }


    // USER
    // BUY NOW

    [HttpPost("buy-now")]
    [Authorize]
    public async Task<IActionResult>
        BuyNow(
            [FromBody]
            BuyNowOrderDto dto)
    {
        try
        {
            var userId =
                GetUserId();

            var result =
                await _orderService
                    .CreateBuyNowOrderAsync(
                        userId,
                        dto);

            return Ok(
                ApiResponse<CheckoutResponseDto>
                    .Success(
                        result,
                        "Order created successfully."));
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
                        "Failed to create order."));
        }
    }


    // USER
    // MY ORDERS

    [HttpGet("my-orders")]
    [Authorize]
    public async Task<IActionResult>
        GetMyOrders()
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


    // USER
    // GET ONE ORDER

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult>
        GetOrderById(
            int id)
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


    // USER
    // CANCEL ORDER

    [HttpPut("{id:int}/cancel")]
    [Authorize]
    public async Task<IActionResult>
        CancelOrder(
            int id)
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


    // ADMIN
    // GET ALL ORDERS

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        GetAllOrders()
    {
        try
        {
            var orders =
                await _orderService
                    .GetAllOrdersAsync();

            return Ok(
                ApiResponse<List<AdminOrderResponseDto>>
                    .Success(
                        orders,
                        "Orders retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<List<AdminOrderResponseDto>>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve orders."));
        }
    }


    // ADMIN
    // GET ONE ORDER

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        GetAdminOrderById(
            int id)
    {
        try
        {
            var order =
                await _orderService
                    .GetAdminOrderByIdAsync(
                        id);

            if (order == null)
            {
                return NotFound(
                    ApiResponse<AdminOrderResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },
                            "Order not found.",
                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<AdminOrderResponseDto>
                    .Success(
                        order,
                        "Order retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<AdminOrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve order."));
        }
    }


    // ADMIN
    // UPDATE STATUS

    [HttpPut("admin/{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        UpdateStatus(
            int id,
            [FromBody]
            UpdateOrderStatusDto dto)
    {
        try
        {
            var order =
                await _orderService
                    .UpdateStatusAsync(
                        id,
                        dto);

            if (order == null)
            {
                return NotFound(
                    ApiResponse<AdminOrderResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },
                            "Order not found.",
                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<AdminOrderResponseDto>
                    .Success(
                        order,
                        "Order status updated successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<AdminOrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to update order status."));
        }
    }


    // ADMIN
    // CANCEL

    [HttpPut("admin/{id:int}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        CancelAdminOrder(
            int id)
    {
        try
        {
            var order =
                await _orderService
                    .CancelAdminOrderAsync(
                        id);

            if (order == null)
            {
                return NotFound(
                    ApiResponse<AdminOrderResponseDto>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },
                            "Order not found.",
                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<AdminOrderResponseDto>
                    .Success(
                        order,
                        "Order cancelled successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<AdminOrderResponseDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to cancel order."));
        }
    }


    // ADMIN
    // DELETE

    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult>
        DeleteOrder(
            int id)
    {
        try
        {
            var deleted =
                await _orderService
                    .DeleteOrderAsync(
                        id);

            if (!deleted)
            {
                return NotFound(
                    ApiResponse<string>
                        .Fail(
                            new List<string>
                            {
                                "Order not found."
                            },
                            "Order not found.",
                            HttpStatusCode.NotFound));
            }


            return Ok(
                ApiResponse<string>
                    .Success(
                        string.Empty,
                        "Order deleted successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<string>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to delete order."));
        }
    }


    // GET USER ID FROM JWT

    private int GetUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        return int.Parse(userId);
    }
}