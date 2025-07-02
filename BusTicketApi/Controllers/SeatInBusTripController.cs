using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using BusTicketApi.Models.Entities;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatInBusTripController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public SeatInBusTripController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetSeats()
    {
        var seats = await _context.SeatsInBusTrip
            .Include(s => s.Trip)
            .ThenInclude(t => t.Route)
            .Include(s => s.Trip)
            .ThenInclude(t => t.Bus)
            .Select(s => new SeatInBusTripResponse
            {
                SeatInTripId = s.SeatInTripId,
                TripId = s.TripId,
                SeatNumber = s.SeatNumber,
                IsAvailable = s.IsAvailable,
                RouteCode = s.Trip.Route.RouteCode,
                BusNumber = s.Trip.Bus.BusNumber,
                DepartureTime = s.Trip.DepartureTime
            })
            .ToListAsync();
        return Ok(seats);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetSeat(int id)
    {
        var seat = await _context.SeatsInBusTrip
            .Include(s => s.Trip)
            .ThenInclude(t => t.Route)
            .Include(s => s.Trip)
            .ThenInclude(t => t.Bus)
            .FirstOrDefaultAsync(s => s.SeatInTripId == id);
        if (seat == null)
        {
            return NotFound($"Seat with ID {id} not found");
        }

        var response = new SeatInBusTripResponse
        {
            SeatInTripId = seat.SeatInTripId,
            TripId = seat.TripId,
            SeatNumber = seat.SeatNumber,
            IsAvailable = seat.IsAvailable,
            RouteCode = seat.Trip.Route.RouteCode,
            BusNumber = seat.Trip.Bus.BusNumber,
            DepartureTime = seat.Trip.DepartureTime
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateSeat([FromBody] SeatInBusTripRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var trip = await _context.BusTrips.FindAsync(request.TripId);
        if (trip == null)
        {
            return BadRequest("Invalid trip");
        }

        var existingSeat = await _context.SeatsInBusTrip
            .FirstOrDefaultAsync(s => s.TripId == request.TripId && s.SeatNumber == request.SeatNumber);
        if (existingSeat != null)
        {
            return BadRequest("Seat number already exists for this trip");
        }

        var seat = new SeatInBusTrip
        {
            TripId = request.TripId,
            SeatNumber = request.SeatNumber,
            IsAvailable = request.IsAvailable
        };

        try
        {
            _context.SeatsInBusTrip.Add(seat);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new SeatInBusTripResponse
        {
            SeatInTripId = seat.SeatInTripId,
            TripId = seat.TripId,
            SeatNumber = seat.SeatNumber,
            IsAvailable = seat.IsAvailable,
            RouteCode = trip.Route.RouteCode,
            BusNumber = trip.Bus.BusNumber,
            DepartureTime = trip.DepartureTime
        };
        return CreatedAtAction(nameof(GetSeat), new { id = seat.SeatInTripId }, response);
    }

    [HttpPost("bulkByTrip")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateSeatsBulkByTrip([FromBody] SeatInBusTripBulkRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var trip = await _context.BusTrips
            .Include(t => t.Bus)
            .Include(t => t.Route)
            .FirstOrDefaultAsync(t => t.TripId == request.TripId);
        if (trip == null)
        {
            return BadRequest("Invalid trip");
        }

        var existingSeats = await _context.SeatsInBusTrip
            .Where(s => s.TripId == request.TripId && request.SeatNumbers.Contains(s.SeatNumber))
            .ToListAsync();
        if (existingSeats.Any())
        {
            return BadRequest($"Seat numbers {string.Join(", ", existingSeats.Select(s => s.SeatNumber))} already exist for this trip");
        }

        var seatsToAdd = new List<SeatInBusTrip>();
        foreach (var seatNumber in request.SeatNumbers)
        {
            seatsToAdd.Add(new SeatInBusTrip
            {
                TripId = request.TripId,
                SeatNumber = seatNumber,
                IsAvailable = true // Mặc định là true
            });
        }

        try
        {
            _context.SeatsInBusTrip.AddRange(seatsToAdd);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = seatsToAdd.Select(s => new SeatInBusTripResponse
        {
            SeatInTripId = s.SeatInTripId,
            TripId = s.TripId,
            SeatNumber = s.SeatNumber,
            IsAvailable = s.IsAvailable,
            RouteCode = trip.Route.RouteCode,
            BusNumber = trip.Bus.BusNumber,
            DepartureTime = trip.DepartureTime
        }).ToList();

        return CreatedAtAction(nameof(GetSeats), response);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatInBusTripRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var seat = await _context.SeatsInBusTrip.FindAsync(id);
        if (seat == null)
        {
            return NotFound($"Seat with ID {id} not found");
        }

        var existingSeat = await _context.SeatsInBusTrip
            .FirstOrDefaultAsync(s => s.TripId == request.TripId && s.SeatNumber == request.SeatNumber && s.SeatInTripId != id);
        if (existingSeat != null)
        {
            return BadRequest("Seat number already exists for this trip");
        }

        var trip = await _context.BusTrips.FindAsync(request.TripId);
        if (trip == null)
        {
            return BadRequest("Invalid trip");
        }

        seat.TripId = request.TripId;
        seat.SeatNumber = request.SeatNumber;
        seat.IsAvailable = request.IsAvailable;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }

        var response = new SeatInBusTripResponse
        {
            SeatInTripId = seat.SeatInTripId,
            TripId = seat.TripId,
            SeatNumber = seat.SeatNumber,
            IsAvailable = seat.IsAvailable,
            RouteCode = trip.Route.RouteCode,
            BusNumber = trip.Bus.BusNumber,
            DepartureTime = trip.DepartureTime
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        var seat = await _context.SeatsInBusTrip.FindAsync(id);
        if (seat == null)
        {
            return NotFound($"Seat with ID {id} not found");
        }

        var hasBookings = await _context.BookingDetails.AnyAsync(bd => bd.SeatInTripId == id);
        if (hasBookings)
        {
            return BadRequest("Cannot delete seat with associated bookings");
        }

        _context.SeatsInBusTrip.Remove(seat);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}