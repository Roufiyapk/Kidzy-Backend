namespace Kidzy.Application.DTOs.Dashboard;

public class DashboardDto
{
    // SUMMARY

    public int TotalUsers { get; set; }

    public int TotalProducts { get; set; }

    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }


    // ORDER STATUS

    public int OrderPlaced { get; set; }

    public int Processing { get; set; }

    public int Shipped { get; set; }

    public int OutForDelivery { get; set; }

    public int Delivered { get; set; }

    public int Cancelled { get; set; }
}