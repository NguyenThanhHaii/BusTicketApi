using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using BusTicketApi.Models.Entities;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BusTicketDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(BusTicketDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Username == request.Username);
        if (employee == null || !BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
        {
            return Unauthorized("Invalid credentials");
        }
        var token = GenerateJwtToken(employee);
        return Ok(new LoginResponse { Token = token, Role = employee.Role });
    }

    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Employees.AnyAsync(e => e.Username == request.Username))
        {
            return BadRequest("Username already exists");
        }
        var employee = new Employee
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Qualifications = request.Qualifications,
            Role = request.Role,
            Age = request.Age,
            Location = request.Location
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return Ok("Employee registered successfully");
    }

    private string GenerateJwtToken(Employee employee)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, employee.Username),
            new Claim(ClaimTypes.Role, employee.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}