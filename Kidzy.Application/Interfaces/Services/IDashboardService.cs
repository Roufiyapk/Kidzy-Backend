using Kidzy.Application.DTOs.Dashboard;

namespace Kidzy.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardDto>
        GetDashboardAsync();
}