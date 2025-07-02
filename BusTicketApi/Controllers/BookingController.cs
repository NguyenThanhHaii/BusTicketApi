using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusTicketApi.Data;
using BusTicketApi.Models.DTOs;
using BusTicketApi.Models.Entities;
using System.Security.Claims;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace BusTicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public BookingController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeOnly")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username);
            if (employee == null)
            {
                return Unauthorized("Invalid employee");
            }

            var seatIds = request.Details.Select(d => d.SeatInTripId).ToList();
            var existingSeats = await _context.BookingDetails
                .Where(bd => seatIds.Contains(bd.SeatInTripId))
                .Select(bd => bd.SeatInTripId)
                .ToListAsync();
            if (existingSeats.Any())
            {
                return BadRequest($"Seats {string.Join(", ", existingSeats)} are already booked");
            }

            var booking = new Booking
            {
                EmployeeId = employee.EmployeeId,
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                TotalAmount = 0,
                TotalTax = 0
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            decimal totalAmount = 0;
            decimal totalTax = 0;

            foreach (var detail in request.Details)
            {
                var seat = await _context.SeatsInBusTrip
                    .FirstOrDefaultAsync(s => s.SeatInTripId == detail.SeatInTripId && s.IsAvailable);
                if (seat == null)
                {
                    return BadRequest($"Seat {detail.SeatInTripId} is not available");
                }

                var trip = await _context.BusTrips
                    .Include(t => t.Bus)
                    .ThenInclude(b => b.Type)
                    .Include(t => t.Route)
                    .FirstOrDefaultAsync(t => t.TripId == seat.TripId);
                if (trip == null)
                {
                    return BadRequest("Invalid trip");
                }

                Customer customer;
                if (detail.CustomerId.HasValue)
                {
                    customer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.CustomerId == detail.CustomerId.Value);
                    if (customer == null)
                    {
                        return BadRequest($"Customer {detail.CustomerId} not found");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(detail.Name) || !detail.DateOfBirth.HasValue)
                    {
                        return BadRequest("Name and DateOfBirth are required for new customers");
                    }
                    customer = new Customer
                    {
                        Name = detail.Name,
                        DateOfBirth = detail.DateOfBirth.Value,
                        Email = detail.Email ?? string.Empty,
                        PhoneNumber = detail.PhoneNumber ?? string.Empty
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                var age = DateTime.Now.Year - customer.DateOfBirth.Year;
                var discount = await _context.AgeBasedDiscounts
                    .FirstOrDefaultAsync(d => age >= d.MinAge && age <= d.MaxAge);
                if (discount == null)
                {
                    return BadRequest($"No discount rule found for age {age}");
                }

                var ticketPrice = trip.Route.BasePrice * trip.Bus.Type.PriceMultiplier * (1 - discount.DiscountPercentage / 100);
                var ticketTax = ticketPrice * 0.1m;

                var bookingDetail = new BookingDetail
                {
                    BookingId = booking.BookingId,
                    SeatInTripId = detail.SeatInTripId,
                    CustomerId = customer.CustomerId,
                    TicketPrice = ticketPrice,
                    TicketTax = ticketTax
                };
                _context.BookingDetails.Add(bookingDetail);
                seat.IsAvailable = false;
                trip.AvailableSeats--;

                totalAmount += ticketPrice;
                totalTax += ticketTax;
            }

            booking.TotalAmount = totalAmount;
            booking.TotalTax = totalTax;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var response = new BookingResponse
            {
                BookingId = booking.BookingId,
                EmployeeId = booking.EmployeeId,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                TotalTax = booking.TotalTax,
                Status = booking.Status,
                Details = request.Details.Select(d => new BookingDetailResponse
                {
                    BookingDetailId = _context.BookingDetails
                        .Where(bd => bd.BookingId == booking.BookingId && bd.SeatInTripId == d.SeatInTripId)
                        .Select(bd => bd.BookingDetailId)
                        .FirstOrDefault(),
                    SeatInTripId = d.SeatInTripId,
                    CustomerId = _context.BookingDetails
                        .Where(bd => bd.BookingId == booking.BookingId && bd.SeatInTripId == d.SeatInTripId)
                        .Select(bd => bd.CustomerId)
                        .FirstOrDefault(),
                    TicketPrice = _context.BookingDetails
                        .Where(bd => bd.BookingId == booking.BookingId && bd.SeatInTripId == d.SeatInTripId)
                        .Select(bd => bd.TicketPrice)
                        .FirstOrDefault(),
                    TicketTax = _context.BookingDetails
                        .Where(bd => bd.BookingId == booking.BookingId && bd.SeatInTripId == d.SeatInTripId)
                        .Select(bd => bd.TicketTax)
                        .FirstOrDefault()
                }).ToList()
            };
            return Ok(response);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return BadRequest($"Database error: {ex.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error creating booking: {ex.Message}");
        }
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Policy = "EmployeeOnly")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Details)
                .ThenInclude(bd => bd.SeatInTrip)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null || booking.Status == "Cancelled")
            {
                return BadRequest("Booking not found or already cancelled");
            }

            var trip = await _context.BusTrips
                .FirstOrDefaultAsync(t => t.TripId == booking.Details.First().SeatInTrip.TripId);
            if (trip == null)
            {
                return BadRequest("Trip not found");
            }

            var daysBefore = (trip.DepartureTime - DateTime.Now).Days;
            var rule = await _context.CancellationRules
                .OrderByDescending(r => r.DaysBeforeDeparture)
                .FirstOrDefaultAsync(r => daysBefore >= r.DaysBeforeDeparture);
            if (rule == null)
            {
                return BadRequest("No cancellation rule found");
            }

            booking.Status = "Cancelled";
            booking.CancellationDate = DateTime.Now;
            booking.RefundAmount = booking.TotalAmount * (1 - rule.PenaltyPercentage / 100);

            foreach (var detail in booking.Details)
            {
                var seat = detail.SeatInTrip;
                seat.IsAvailable = true;
            }
            trip.AvailableSeats += booking.Details.Count;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new CancellationResponse
            {
                BookingId = booking.BookingId,
                CancellationDate = booking.CancellationDate.Value,
                RefundAmount = booking.RefundAmount.Value,
                Status = booking.Status
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Error cancelling booking");
        }
    }

    [HttpGet("{id}/ticket")]
    [Authorize(Policy = "EmployeeOnly")]
    public async Task<IActionResult> GetTicket(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Details)
            .ThenInclude(bd => bd.Customer)
            .Include(b => b.Details)
            .ThenInclude(bd => bd.SeatInTrip)
            .ThenInclude(s => s.Trip)
            .ThenInclude(t => t.Route)
            .Include(b => b.Details)
            .ThenInclude(bd => bd.SeatInTrip)
            .ThenInclude(s => s.Trip)
            .ThenInclude(t => t.Bus)
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null)
        {
            return NotFound($"Booking with ID {id} not found");
        }

        try
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");
            document.Add(new Paragraph("Bus Ticket")
                .SetFont(boldFont)
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER));

            var regularFont = PdfFontFactory.CreateFont("Helvetica");
            document.Add(new Paragraph($"Booking ID: {booking.BookingId}")
                .SetFont(regularFont));
            document.Add(new Paragraph($"Booking Date: {booking.BookingDate:yyyy-MM-dd HH:mm:ss}")
                .SetFont(regularFont));
            document.Add(new Paragraph($"Status: {booking.Status}")
                .SetFont(regularFont));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
            table.AddHeaderCell("Customer");
            table.AddHeaderCell("Details");

            foreach (var detail in booking.Details)
            {
                var customerInfo = $"{detail.Customer.Name}\n" +
                                  $"DOB: {detail.Customer.DateOfBirth:yyyy-MM-dd}\n" +
                                  $"Email: {detail.Customer.Email}\n" +
                                  $"Phone: {detail.Customer.PhoneNumber}";
                var ticketInfo = $"Route: {detail.SeatInTrip.Trip.Route.StartLocation} - {detail.SeatInTrip.Trip.Route.EndLocation}\n" +
                                 $"Bus: {detail.SeatInTrip.Trip.Bus.BusNumber}\n" +
                                 $"Seat: {detail.SeatInTrip.SeatNumber}\n" +
                                 $"Departure: {detail.SeatInTrip.Trip.DepartureTime:yyyy-MM-dd HH:mm:ss}\n" +
                                 $"Price: ${detail.TicketPrice:F2}\n" +
                                 $"Tax: ${detail.TicketTax:F2}";
                table.AddCell(customerInfo);
                table.AddCell(ticketInfo);
            }

            document.Add(table);

            document.Add(new Paragraph($"Total Amount: ${booking.TotalAmount:F2}")
                .SetFont(regularFont)
                .SetTextAlignment(TextAlignment.RIGHT));
            document.Add(new Paragraph($"Total Tax: ${booking.TotalTax:F2}")
                .SetFont(regularFont)
                .SetTextAlignment(TextAlignment.RIGHT));
            if (booking.Status == "Cancelled" && booking.RefundAmount.HasValue)
            {
                document.Add(new Paragraph($"Refund Amount: ${booking.RefundAmount:F2}")
                    .SetFont(regularFont)
                    .SetTextAlignment(TextAlignment.RIGHT));
            }

            document.Close();

            var pdfBytes = memoryStream.ToArray();
            return File(pdfBytes, "application/pdf", $"Ticket_{booking.BookingId}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error generating ticket PDF: {ex.Message}");
        }
    }
}