using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public ReportController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpGet("bookings")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBookingReport()
    {
        var reports = await _context.Bookings
            .GroupBy(b => new { Year = b.BookingDate.Year, Month = b.BookingDate.Month })
            .Select(g => new ReportResponse
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                TotalTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month),
                TotalRevenue = g.Sum(b => b.TotalAmount),
                TotalTax = g.Sum(b => b.TotalTax),
                CancelledTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month && bd.Booking.Status == "Cancelled"),
                TotalRefund = g.Where(b => b.Status == "Cancelled").Sum(b => b.RefundAmount ?? 0)
            })
            .OrderBy(r => r.Period)
            .ToListAsync();
        return Ok(reports);
    }

    [HttpGet("bookings/{year}/{month}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBookingReportByMonth(int year, int month)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest("Invalid month");
        }

        var report = await _context.Bookings
            .Where(b => b.BookingDate.Year == year && b.BookingDate.Month == month)
            .GroupBy(b => new { Year = b.BookingDate.Year, Month = b.BookingDate.Month })
            .Select(g => new ReportResponse
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                TotalTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month),
                TotalRevenue = g.Sum(b => b.TotalAmount),
                TotalTax = g.Sum(b => b.TotalTax),
                CancelledTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month && bd.Booking.Status == "Cancelled"),
                TotalRefund = g.Where(b => b.Status == "Cancelled").Sum(b => b.RefundAmount ?? 0)
            })
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return NotFound($"No bookings found for {year}-{month:D2}");
        }

        return Ok(report);
    }

    [HttpGet("bookings/{year}/{month}/{day}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetBookingReportByDay(int year, int month, int day)
    {
        if (month < 1 || month > 12 || day < 1 || day > 31)
        {
            return BadRequest("Invalid date");
        }

        var report = await _context.Bookings
            .Where(b => b.BookingDate.Year == year && b.BookingDate.Month == month && b.BookingDate.Day == day)
            .GroupBy(b => new { Year = b.BookingDate.Year, Month = b.BookingDate.Month, Day = b.BookingDate.Day })
            .Select(g => new ReportResponse
            {
                Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}",
                TotalTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month && bd.Booking.BookingDate.Day == g.Key.Day),
                TotalRevenue = g.Sum(b => b.TotalAmount),
                TotalTax = g.Sum(b => b.TotalTax),
                CancelledTickets = _context.BookingDetails
                    .Count(bd => bd.Booking.BookingDate.Year == g.Key.Year && bd.Booking.BookingDate.Month == g.Key.Month && bd.Booking.BookingDate.Day == g.Key.Day && bd.Booking.Status == "Cancelled"),
                TotalRefund = g.Where(b => b.Status == "Cancelled").Sum(b => b.RefundAmount ?? 0)
            })
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return NotFound($"No bookings found for {year}-{month:D2}-{day:D2}");
        }

        return Ok(report);
    }
}