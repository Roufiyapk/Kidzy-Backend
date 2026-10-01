using System.Net;
using Kidzy.API.Contracts;
using Kidzy.Application.DTOs.Dashboard;
using Kidzy.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kidzy.API.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class DashboardController
    : ControllerBase
{
    private readonly IDashboardService
        _dashboardService;

    public DashboardController(
        IDashboardService dashboardService)
    {
        _dashboardService =
            dashboardService;
    }


    // GET ADMIN DASHBOARD

    [HttpGet]
    public async Task<IActionResult>
        GetDashboard()
    {
        try
        {
            var dashboard =
                await _dashboardService
                    .GetDashboardAsync();

            return Ok(
                ApiResponse<DashboardDto>
                    .Success(
                        dashboard,
                        "Dashboard data retrieved successfully."));
        }
        catch (Exception ex)
        {
            return BadRequest(
                ApiResponse<DashboardDto>
                    .Fail(
                        new List<string>
                        {
                            ex.Message
                        },
                        "Failed to retrieve dashboard data.",
                        HttpStatusCode.BadRequest));
        }
    }
}