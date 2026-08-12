using System.Security.Claims;
using EmpTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(IDashboardRepository dashRepo) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = CurrentRole == "Admin" ? (int?)null : CurrentUserId;
        return Ok(await dashRepo.GetStatsAsync(userId, CurrentRole));
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent()
    {
        var userId = CurrentRole == "Admin" ? (int?)null : CurrentUserId;
        return Ok(await dashRepo.GetRecentTasksAsync(userId, CurrentRole));
    }
}
