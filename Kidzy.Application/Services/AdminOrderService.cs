using Kidzy.Application.DTOs.Admin;
using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;
using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class AdminOrderService
    : IAdminOrderService
{
    private readonly IAdminOrderRepository
        _adminOrderRepository;

    public AdminOrderService(
        IAdminOrderRepository adminOrderRepository)
    {
        _adminOrderRepository =
            adminOrderRepository;
    }


    // GET ALL ORDERS

    public async Task<List<AdminOrderResponseDto>>
        GetAllOrdersAsync()
    {
        var orders =
            await _adminOrderRepository
                .GetAllOrdersAsync();

        return orders
            .Select(MapToDto)
            .ToList();
    }


    // GET ONE ORDER

    public async Task<AdminOrderResponseDto?>
        GetOrderByIdAsync(
            int orderId)
    {
        var order =
            await _adminOrderRepository
                .GetOrderByIdAsync(
                    orderId);

        if (order == null)
            return null;

        return MapToDto(order);
    }


    // UPDATE STATUS
    

    public async Task<AdminOrderResponseDto?>
        UpdateStatusAsync(
            int orderId,
            UpdateOrderStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(
                dto.Status))
        {
            throw new Exception(
                "Order status is required.");
        }


        var order =
            await _adminOrderRepository
                .GetOrderByIdAsync(
                    orderId);

        if (order == null)
            return null;


        var newStatus =
            ParseStatus(dto.Status);


        var currentStatus =
            order.Status;


        // Already cancelled
        // cannot be reopened.

        if (currentStatus ==
                OrderStatus.Cancelled &&
            newStatus !=
                OrderStatus.Cancelled)
        {
            throw new Exception(
                "Cancelled order cannot be reopened.");
        }


        // If admin changes status to Cancelled,
        // use cancellation logic.

        if (newStatus ==
                OrderStatus.Cancelled &&
            currentStatus !=
                OrderStatus.Cancelled)
        {
            if (currentStatus !=
                    OrderStatus.OrderPlaced &&
                currentStatus !=
                    OrderStatus.Processing)
            {
                throw new Exception(
                    "This order cannot be cancelled at this stage.");
            }


            RestoreStock(order);

            order.Status =
                OrderStatus.Cancelled;

            order.CancelledAt =
                DateTime.UtcNow;
        }
        else
        {
            order.Status =
                newStatus;
        }


        await _adminOrderRepository
            .SaveChangesAsync();

        return MapToDto(order);
    }


    // CANCEL ORDER

    public async Task<AdminOrderResponseDto?>
        CancelOrderAsync(
            int orderId)
    {
        var order =
            await _adminOrderRepository
                .GetOrderByIdAsync(
                    orderId);

        if (order == null)
            return null;


        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        // Restore stock

        RestoreStock(order);


        order.Status =
            OrderStatus.Cancelled;

        order.CancelledAt =
            DateTime.UtcNow;


        await _adminOrderRepository
            .SaveChangesAsync();


        return MapToDto(order);
    }


    // DELETE ORDER

    public async Task<bool>
        DeleteOrderAsync(
            int orderId)
    {
        var order =
            await _adminOrderRepository
                .GetOrderByIdAsync(
                    orderId);

        if (order == null)
            return false;


        // Only cancelled orders can be deleted.

        if (order.Status !=
            OrderStatus.Cancelled)
        {
            throw new Exception(
                "Only cancelled orders can be deleted.");
        }


        _adminOrderRepository
            .Delete(order);


        await _adminOrderRepository
            .SaveChangesAsync();


        return true;
    }


    // RESTORE STOCK

    private static void RestoreStock(
        Order order)
    {
        foreach (var item in order.Items)
        {
            if (item.ProductVariant != null)
            {
                item.ProductVariant.Stock +=
                    item.Quantity;
            }
            else if (item.Product != null)
            {
                item.Product.Stock +=
                    item.Quantity;
            }
        }
    }


    // PARSE STATUS

    private static OrderStatus
        ParseStatus(
            string status)
    {
        var normalized =
            status
                .Trim()
                .Replace(" ", "")
                .ToLowerInvariant();


        return normalized switch
        {
            "orderplaced" =>
                OrderStatus.OrderPlaced,

            "processing" =>
                OrderStatus.Processing,

            "shipped" =>
                OrderStatus.Shipped,

            "outfordelivery" =>
                OrderStatus.OutForDelivery,

            "delivered" =>
                OrderStatus.Delivered,

            "cancelled" =>
                OrderStatus.Cancelled,

            _ =>
                throw new Exception(
                    "Invalid order status.")
        };
    }


    // MAP

    private static AdminOrderResponseDto
        MapToDto(
            Order order)
    {
        return new AdminOrderResponseDto
        {
            Id =
                order.Id,

            UserId =
                order.UserId,

            UserName =
                order.User?.Name
                ?? string.Empty,

            UserEmail =
                order.User?.Email
                ?? string.Empty,

            Subtotal =
                order.Subtotal,

            DeliveryFee =
                order.DeliveryFee,

            TotalAmount =
                order.TotalAmount,

            Name =
                order.ShippingName,

            Phone =
                order.ShippingPhone,

            Address =
                order.ShippingAddress,

            Pincode =
                order.ShippingPincode,

            PaymentMethod =
                order.PaymentMethod,

            PaymentStatus =
                order.PaymentStatus,

            RazorpayPaymentId =
                order.RazorpayPaymentId,

            RazorpayOrderId =
                order.RazorpayOrderId,

            Status =
                GetStatusText(
                    order.Status),

            CreatedAt =
                order.CreatedAt,

            CancelledAt =
                order.CancelledAt,

            Items =
                order.Items
                    .Select(
                        item =>
                            new OrderItemDto
                            {
                                Id =
                                    item.Id,

                                ProductId =
                                    item.ProductId,

                                ProductVariantId =
                                    item.ProductVariantId,

                                ProductName =
                                    item.ProductName,

                                AgeGroup =
                                    item.AgeGroup,

                                Size =
                                    item.Size,

                                Price =
                                    item.Price,

                                Quantity =
                                    item.Quantity,

                                TotalPrice =
                                    item.TotalPrice
                            })
                    .ToList()
        };
    }


    // STATUS TEXT

    private static string
        GetStatusText(
            OrderStatus status)
    {
        return status switch
        {
            OrderStatus.OrderPlaced =>
                "Order Placed",

            OrderStatus.Processing =>
                "Processing",

            OrderStatus.Shipped =>
                "Shipped",

            OrderStatus.OutForDelivery =>
                "Out for Delivery",

            OrderStatus.Delivered =>
                "Delivered",

            OrderStatus.Cancelled =>
                "Cancelled",

            _ =>
                "Order Placed"
        };
    }
}