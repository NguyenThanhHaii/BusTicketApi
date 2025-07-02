using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using Route = BusTicketApi.Models.Entities.Route;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public RouteController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetRoutes()
    {
        var routes = await _context.Routes
            .Select(r => new RouteResponse
            {
                RouteId = r.RouteId,
                RouteCode = r.RouteCode,
                StartLocation = r.StartLocation,
                EndLocation = r.EndLocation,
                Distance = r.Distance,
                BasePrice = r.BasePrice
            })
            .ToListAsync();
        return Ok(routes);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteId == id);
        if (route == null)
        {
            return NotFound($"Route with ID {id} not found");
        }

        var response = new RouteResponse
        {
            RouteId = route.RouteId,
            RouteCode = route.RouteCode,
            StartLocation = route.StartLocation,
            EndLocation = route.EndLocation,
            Distance = route.Distance,
            BasePrice = route.BasePrice
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateRoute([FromBody] RouteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingRoute = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteCode == request.RouteCode);
        if (existingRoute != null)
        {
            return BadRequest("Route code already exists");
        }

        var route = new Route
        {
            RouteCode = request.RouteCode,
            StartLocation = request.StartLocation,
            EndLocation = request.EndLocation,
            Distance = request.Distance,
            BasePrice = request.BasePrice
        };

        try
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new RouteResponse
        {
            RouteId = route.RouteId,
            RouteCode = route.RouteCode,
            StartLocation = route.StartLocation,
            EndLocation = route.EndLocation,
            Distance = route.Distance,
            BasePrice = route.BasePrice
        };
        return CreatedAtAction(nameof(GetRoute), new { id = route.RouteId }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var route = await _context.Routes.FindAsync(id);
        if (route == null)
        {
            return NotFound($"Route with ID {id} not found");
        }

        var existingRoute = await _context.Routes
            .FirstOrDefaultAsync(r => r.RouteCode == request.RouteCode && r.RouteId != id);
        if (existingRoute != null)
        {
            return BadRequest("Route code already exists");
        }

        route.RouteCode = request.RouteCode;
        route.StartLocation = request.StartLocation;
        route.EndLocation = request.EndLocation;
        route.Distance = request.Distance;
        route.BasePrice = request.BasePrice;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new RouteResponse
        {
            RouteId = route.RouteId,
            RouteCode = route.RouteCode,
            StartLocation = route.StartLocation,
            EndLocation = route.EndLocation,
            Distance = route.Distance,
            BasePrice = route.BasePrice
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteRoute(int id)
    {
        var route = await _context.Routes.FindAsync(id);
        if (route == null)
        {
            return NotFound($"Route with ID {id} not found");
        }

        var hasTrips = await _context.BusTrips.AnyAsync(t => t.RouteId == id);
        if (hasTrips)
        {
            return BadRequest("Cannot delete route with associated trips");
        }

        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}