using Kidzy.Application.DTOs.Orders;
using Kidzy.Application.Interfaces.Repositories;
using Kidzy.Application.Interfaces.Services;

using Kidzy.Domain.Entities;
using Kidzy.Domain.Enums;

namespace Kidzy.Application.Services;

public class OrderService
    : IOrderService
{
    private readonly IOrderRepository
        _orderRepository;


    public OrderService(
        IOrderRepository orderRepository)
    {
        _orderRepository =
            orderRepository;
    }


    // USER
    // CREATE ORDER FROM CART

    public async Task<CheckoutResponseDto>
        CreateOrderFromCartAsync(
            int userId,
            CreateOrderDto dto)
    {
       

        throw new NotImplementedException(
            "Use your existing CreateOrderFromCartAsync logic here.");
    }


    // USER
    // BUY NOW

    public async Task<CheckoutResponseDto>
        CreateBuyNowOrderAsync(
            int userId,
            BuyNowOrderDto dto)
    {
        

        throw new NotImplementedException(
            "Use your existing CreateBuyNowOrderAsync logic here.");
    }


    // USER
    // GET MY ORDERS

    public async Task<List<OrderResponseDto>>
        GetUserOrdersAsync(
            int userId)
    {
        var orders =
            await _orderRepository
                .GetUserOrdersAsync(
                    userId);


        return orders
            .Select(MapToUserDto)
            .ToList();
    }


    // USER
    // GET ONE ORDER

    public async Task<OrderResponseDto?>
        GetOrderByIdAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
            return null;


        // User can only see his own order.

        if (order.UserId != userId)
            return null;


        return MapToUserDto(order);
    }


    // USER
    // CANCEL ORDER

    public async Task<OrderResponseDto?>
        CancelOrderAsync(
            int userId,
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
            return null;


        if (order.UserId != userId)
            return null;


        if (order.Status !=
                OrderStatus.OrderPlaced &&
            order.Status !=
                OrderStatus.Processing)
        {
            throw new Exception(
                "This order cannot be cancelled.");
        }


        RestoreStock(order);


        order.Status =
            OrderStatus.Cancelled;


        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToUserDto(order);
    }


    // ADMIN
    // GET ALL ORDERS
    // FILTER BY STATUS

    public async Task<List<AdminOrderResponseDto>>
        GetAllOrdersAsync(
            string? status = null)
    {
        OrderStatus? orderStatus = null;


        // If status was provided,
        // convert string to enum.

        if (!string.IsNullOrWhiteSpace(status))
        {
            orderStatus =
                ParseStatus(status);
        }


        var orders =
            await _orderRepository
                .GetAllOrdersAsync(
                    orderStatus);


        return orders
            .Select(MapToAdminDto)
            .ToList();
    }


    // ADMIN
    // GET ONE ORDER

    public async Task<AdminOrderResponseDto?>
        GetAdminOrderByIdAsync(
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
            return null;


        return MapToAdminDto(order);
    }


    // ADMIN
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
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
            return null;


        var newStatus =
            ParseStatus(dto.Status);


        var currentStatus =
            order.Status;


        // CANCELLED ORDER CANNOT BE REOPENED

        if (currentStatus ==
                OrderStatus.Cancelled &&
            newStatus !=
                OrderStatus.Cancelled)
        {
            throw new Exception(
                "Cancelled order cannot be reopened.");
        }


        // CHANGE STATUS TO CANCELLED

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


        await _orderRepository
            .SaveChangesAsync();


        return MapToAdminDto(order);
    }


    // ADMIN
    // CANCEL ORDER

    public async Task<AdminOrderResponseDto?>
        CancelAdminOrderAsync(
            int orderId)
    {
        var order =
            await _orderRepository
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


        RestoreStock(order);


        order.Status =
            OrderStatus.Cancelled;


        order.CancelledAt =
            DateTime.UtcNow;


        await _orderRepository
            .SaveChangesAsync();


        return MapToAdminDto(order);
    }


    
    // ADMIN
    // DELETE ORDER

    public async Task<bool>
        DeleteOrderAsync(
            int orderId)
    {
        var order =
            await _orderRepository
                .GetOrderByIdAsync(
                    orderId);


        if (order == null)
            return false;


        // Only cancelled orders
        // can be deleted.

        if (order.Status !=
            OrderStatus.Cancelled)
        {
            throw new Exception(
                "Only cancelled orders can be deleted.");
        }


        _orderRepository
            .Delete(order);


        await _orderRepository
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


    
    // USER DTO MAPPING

    private static OrderResponseDto
        MapToUserDto(
            Order order)
    {
        return new OrderResponseDto
        {
            Id =
                order.Id,

            UserId =
                order.UserId,

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


    // ADMIN DTO MAPPING

    private static AdminOrderResponseDto
        MapToAdminDto(
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