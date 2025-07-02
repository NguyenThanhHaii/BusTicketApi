using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using BusTicketApi.Models.Entities;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public BusController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBuses()
    {
        var buses = await _context.Buses
            .Include(b => b.Type)
            .Select(b => new BusResponse
            {
                BusId = b.BusId,
                BusCode = b.BusCode,
                BusNumber = b.BusNumber,
                TypeId = b.TypeId,
                TypeName = b.Type.TypeName,
                PriceMultiplier = b.Type.PriceMultiplier,
                TotalSeats = b.TotalSeats
            })
            .ToListAsync();
        return Ok(buses);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBus(int id)
    {
        var bus = await _context.Buses
            .Include(b => b.Type)
            .FirstOrDefaultAsync(b => b.BusId == id);
        if (bus == null)
        {
            return NotFound($"Bus with ID {id} not found");
        }

        var response = new BusResponse
        {
            BusId = bus.BusId,
            BusCode = bus.BusCode,
            BusNumber = bus.BusNumber,
            TypeId = bus.TypeId,
            TypeName = bus.Type.TypeName,
            PriceMultiplier = bus.Type.PriceMultiplier,
            TotalSeats = bus.TotalSeats
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateBus([FromBody] BusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var busType = await _context.BusTypes.FindAsync(request.TypeId);
        if (busType == null)
        {
            return BadRequest("Invalid bus type");
        }

        var existingBus = await _context.Buses
            .FirstOrDefaultAsync(b => b.BusCode == request.BusCode || b.BusNumber == request.BusNumber);
        if (existingBus != null)
        {
            return BadRequest("Bus code or number already exists");
        }

        var bus = new Bus
        {
            BusCode = request.BusCode,
            BusNumber = request.BusNumber,
            TypeId = request.TypeId,
            TotalSeats = request.TotalSeats
        };
        _context.Buses.Add(bus);
        await _context.SaveChangesAsync();

        var response = new BusResponse
        {
            BusId = bus.BusId,
            BusCode = bus.BusCode,
            BusNumber = bus.BusNumber,
            TypeId = bus.TypeId,
            TypeName = busType.TypeName,
            PriceMultiplier = busType.PriceMultiplier,
            TotalSeats = bus.TotalSeats
        };
        return CreatedAtAction(nameof(GetBus), new { id = bus.BusId }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateBus(int id, [FromBody] BusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var bus = await _context.Buses.FindAsync(id);
        if (bus == null)
        {
            return NotFound($"Bus with ID {id} not found");
        }

        var busType = await _context.BusTypes.FindAsync(request.TypeId);
        if (busType == null)
        {
            return BadRequest("Invalid bus type");
        }

        var existingBus = await _context.Buses
            .FirstOrDefaultAsync(b => (b.BusCode == request.BusCode || b.BusNumber == request.BusNumber) && b.BusId != id);
        if (existingBus != null)
        {
            return BadRequest("Bus code or number already exists");
        }

        bus.BusCode = request.BusCode;
        bus.BusNumber = request.BusNumber;
        bus.TypeId = request.TypeId;
        bus.TotalSeats = request.TotalSeats;
        await _context.SaveChangesAsync();

        var response = new BusResponse
        {
            BusId = bus.BusId,
            BusCode = bus.BusCode,
            BusNumber = bus.BusNumber,
            TypeId = bus.TypeId,
            TypeName = busType.TypeName,
            PriceMultiplier = busType.PriceMultiplier,
            TotalSeats = bus.TotalSeats
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteBus(int id)
    {
        var bus = await _context.Buses.FindAsync(id);
        if (bus == null)
        {
            return NotFound($"Bus with ID {id} not found");
        }

        var hasTrips = await _context.BusTrips.AnyAsync(t => t.BusId == id);
        if (hasTrips)
        {
            return BadRequest("Cannot delete bus with associated trips");
        }

        _context.Buses.Remove(bus);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}