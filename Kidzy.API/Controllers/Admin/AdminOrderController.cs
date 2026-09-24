using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrderController
    : ControllerBase
{
    private readonly IAdminOrderService
        _adminOrderService;

    public AdminOrderController(
        IAdminOrderService adminOrderService)
    {
        _adminOrderService =
            adminOrderService;
    }


    // GET ALL ORDERS

    [HttpGet]
    public async Task<IActionResult>
        GetAllOrders()
    {
        try
        {
            var orders =
                await _adminOrderService
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


    // GET ONE ORDER

    [HttpGet("{id:int}")]
    public async Task<IActionResult>
        GetOrderById(
            int id)
    {
        try
        {
            var order =
                await _adminOrderService
                    .GetOrderByIdAsync(id);

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


    // UPDATE STATUS

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult>
        UpdateStatus(
            int id,
            [FromBody]
            UpdateOrderStatusDto dto)
    {
        try
        {
            var order =
                await _adminOrderService
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


    // =====================================================
    // CANCEL
    // PUT /api/admin/orders/{id}/cancel
    // =====================================================

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult>
        CancelOrder(
            int id)
    {
        try
        {
            var order =
                await _adminOrderService
                    .CancelOrderAsync(id);

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


    // DELETE

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteOrder(
            int id)
    {
        try
        {
            var deleted =
                await _adminOrderService
                    .DeleteOrderAsync(id);

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
}