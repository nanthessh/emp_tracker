using EmpTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserRepository userRepo) : ControllerBase
{
    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees()
        => Ok(await userRepo.GetAllEmployeesAsync());
}
