using EmpTracker.API.Services;
using EmpTracker.Core.DTOs;
using EmpTracker.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmpTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepo, JwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userRepo.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = jwtService.GenerateToken(user);
        return Ok(new LoginResponse { Token = token, Name = user.Name, Role = user.Role, UserId = user.UserId });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await userRepo.GetByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest(new { message = "Email already registered." });

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var id = await userRepo.CreateAsync(request.Name, request.Email, hash, request.Role);
        return Ok(new { UserId = id });
    }
}
