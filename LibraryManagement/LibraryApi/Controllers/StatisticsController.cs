using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Reports.View")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService) =>
        _statisticsService = statisticsService;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard() =>
        Ok(await _statisticsService.GetDashboardAsync());

    [HttpGet("loans")]
    public async Task<IActionResult> GetLoans() =>
        Ok(await _statisticsService.GetLoanStatisticsAsync());

    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions() =>
        Ok(await _statisticsService.GetSubscriptionStatisticsAsync());

    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancial() =>
        Ok(await _statisticsService.GetFinancialStatisticsAsync());

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly([FromQuery] int months = 12)
    {
        try { return Ok(await _statisticsService.GetMonthlyStatisticsAsync(months)); }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
