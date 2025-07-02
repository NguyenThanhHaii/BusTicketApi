using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public Task<IActionResult> GetBookingReport()
    {
        var reports = (from b in _context.Bookings
                      join bd in _context.BookingDetails on b.BookingId equals bd.BookingId into bookingDetails
                      from bd in bookingDetails.DefaultIfEmpty()
                      group new { b, bd } by new { Year = b.BookingDate.Year, Month = b.BookingDate.Month } into g
                      select new
                      {
                          Key = g.Key,
                          TotalRevenue = g.Sum(x => x.b.TotalAmount),
                          TotalTax = g.Sum(x => x.b.TotalTax),
                          TotalTickets = g.Count(x => x.bd != null),
                          CancelledTickets = g.Count(x => x.b.Status == "Cancelled" && x.bd != null),
                          TotalRefund = g.Where(x => x.b.Status == "Cancelled").Sum(x => x.b.RefundAmount ?? 0)
                      })
                      .AsEnumerable()
                      .Select(g => new ReportResponse
                      {
                          Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                          TotalTickets = g.TotalTickets,
                          TotalRevenue = g.TotalRevenue,
                          TotalTax = g.TotalTax,
                          CancelledTickets = g.CancelledTickets,
                          TotalRefund = g.TotalRefund
                      })
                      .OrderBy(r => r.Period)
                      .ToList();

        return Task.FromResult<IActionResult>(Ok(reports));
    }

    [HttpGet("bookings/{year}/{month}")]
    [Authorize(Policy = "AdminOnly")]
    public Task<IActionResult> GetBookingReportByMonth(int year, int month)
    {
        if (month is < 1 or > 12)
        {
            return Task.FromResult<IActionResult>(BadRequest("Invalid month"));
        }

        var report = (from b in _context.Bookings
                     where b.BookingDate.Year == year && b.BookingDate.Month == month
                     join bd in _context.BookingDetails on b.BookingId equals bd.BookingId into bookingDetails
                     from bd in bookingDetails.DefaultIfEmpty()
                     group new { b, bd } by new { Year = b.BookingDate.Year, Month = b.BookingDate.Month } into g
                     select new
                     {
                         Key = g.Key,
                         TotalRevenue = g.Sum(x => x.b.TotalAmount),
                         TotalTax = g.Sum(x => x.b.TotalTax),
                         TotalTickets = g.Count(x => x.bd != null),
                         CancelledTickets = g.Count(x => x.b.Status == "Cancelled" && x.bd != null),
                         TotalRefund = g.Where(x => x.b.Status == "Cancelled").Sum(x => x.b.RefundAmount ?? 0)
                     })
                     .AsEnumerable()
                     .Select(g => new ReportResponse
                     {
                         Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                         TotalTickets = g.TotalTickets,
                         TotalRevenue = g.TotalRevenue,
                         TotalTax = g.TotalTax,
                         CancelledTickets = g.CancelledTickets,
                         TotalRefund = g.TotalRefund
                     })
                     .FirstOrDefault();

        return report == null ? Task.FromResult<IActionResult>(NotFound($"No bookings found for {year}-{month:D2}")) : Task.FromResult<IActionResult>(Ok(report));
    }

    [HttpGet("bookings/{year}/{month}/{day}")]
    [Authorize(Policy = "AdminOnly")]
    public Task<IActionResult> GetBookingReportByDay(int year, int month, int day)
    {
        if (month < 1 || month > 12 || day < 1 || day > 31)
        {
            return Task.FromResult<IActionResult>(BadRequest("Invalid date"));
        }

        var report = (from b in _context.Bookings
                     where b.BookingDate.Year == year && b.BookingDate.Month == month && b.BookingDate.Day == day
                     join bd in _context.BookingDetails on b.BookingId equals bd.BookingId into bookingDetails
                     from bd in bookingDetails.DefaultIfEmpty()
                     group new { b, bd } by new { Year = b.BookingDate.Year, Month = b.BookingDate.Month, Day = b.BookingDate.Day } into g
                     select new
                     {
                         Key = g.Key,
                         TotalRevenue = g.Sum(x => x.b.TotalAmount),
                         TotalTax = g.Sum(x => x.b.TotalTax),
                         TotalTickets = g.Count(x => x.bd != null),
                         CancelledTickets = g.Count(x => x.b.Status == "Cancelled" && x.bd != null),
                         TotalRefund = g.Where(x => x.b.Status == "Cancelled").Sum(x => x.b.RefundAmount ?? 0)
                     })
                     .AsEnumerable()
                     .Select(g => new ReportResponse
                     {
                         Period = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}",
                         TotalTickets = g.TotalTickets,
                         TotalRevenue = g.TotalRevenue,
                         TotalTax = g.TotalTax,
                         CancelledTickets = g.CancelledTickets,
                         TotalRefund = g.TotalRefund
                     })
                     .FirstOrDefault();

        return report == null ? Task.FromResult<IActionResult>(NotFound($"No bookings found for {year}-{month:D2}-{day:D2}")) : Task.FromResult<IActionResult>(Ok(report));
    }
}