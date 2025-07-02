using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class SeatInBusTripBulkRequest
{
    [Required]
    public int TripId { get; set; }
    [Required]
    public List<string> SeatNumbers { get; set; }
}