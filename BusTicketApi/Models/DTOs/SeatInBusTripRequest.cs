using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class SeatInBusTripRequest
{
    [Required]
    public int TripId { get; set; }
    [Required]
    [StringLength(10, ErrorMessage = "SeatNumber must not exceed 10 characters")]
    public string SeatNumber { get; set; }
    [Required]
    public bool IsAvailable { get; set; }
}