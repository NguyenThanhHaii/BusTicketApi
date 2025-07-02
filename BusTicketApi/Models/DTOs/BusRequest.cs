using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class BusRequest
{
    [Required]
    public string BusCode { get; set; }
    [Required]
    public string BusNumber { get; set; }
    [Required]
    public int TypeId { get; set; }
    [Required]
    [Range(1, 100, ErrorMessage = "TotalSeats must be between 1 and 100")]
    public int TotalSeats { get; set; }
}