using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class BookingRequest
{
    [Required]
    public List<BookingDetailRequest> Details { get; set; }
}

public class BookingDetailRequest
{
    public int? CustomerId { get; set; }
    public int SeatInTripId { get; set; }
    public string? Name { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}