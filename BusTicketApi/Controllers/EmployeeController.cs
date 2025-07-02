using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using BusTicketApi.Models.Entities;
using System.Security.Claims;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public EmployeeController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .Select(e => new EmployeeResponse
            {
                EmployeeId = e.EmployeeId,
                Username = e.Username,
                Name = e.Name,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Qualifications = e.Qualifications,
                Role = e.Role,
                Age = e.Age,
                Location = e.Location
            })
            .ToListAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found");
        }

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            Username = employee.Username,
            Name = employee.Name,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Qualifications = employee.Qualifications,
            Role = employee.Role,
            Age = employee.Age,
            Location = employee.Location
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Username == request.Username || e.Email == request.Email);
        if (existingEmployee != null)
        {
            return BadRequest("Username or email already exists");
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

        try
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            Username = employee.Username,
            Name = employee.Name,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Qualifications = employee.Qualifications,
            Role = employee.Role,
            Age = employee.Age,
            Location = employee.Location
        };
        return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found");
        }

        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => (e.Username == request.Username || e.Email == request.Email) && e.EmployeeId != id);
        if (existingEmployee != null)
        {
            return BadRequest("Username or email already exists");
        }

        employee.Username = request.Username;
        employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        employee.Name = request.Name;
        employee.Email = request.Email;
        employee.PhoneNumber = request.PhoneNumber;
        employee.Qualifications = request.Qualifications;
        employee.Role = request.Role;
        employee.Age = request.Age;
        employee.Location = request.Location;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new EmployeeResponse
        {
            EmployeeId = employee.EmployeeId,
            Username = employee.Username,
            Name = employee.Name,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            Qualifications = employee.Qualifications,
            Role = employee.Role,
            Age = employee.Age,
            Location = employee.Location
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found");
        }

        var currentUser = User.FindFirst(ClaimTypes.Name)?.Value;
        var currentEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Username == currentUser);
        if (currentEmployee?.EmployeeId == id)
        {
            return BadRequest("Cannot delete your own account");
        }

        var hasBookings = await _context.Bookings.AnyAsync(b => b.EmployeeId == id);
        if (hasBookings)
        {
            return BadRequest("Cannot delete employee with associated bookings");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}