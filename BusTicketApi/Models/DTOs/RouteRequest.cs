using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class RouteRequest
{
    [Required]
    [RegularExpression(@"^[A-Z]{2}-[A-Z]{2,3}$", ErrorMessage = "RouteCode must be in format 'XX-YYY' (e.g., NY-BOS)")]
    public string RouteCode { get; set; }
    [Required]
    public string StartLocation { get; set; }
    [Required]
    public string EndLocation { get; set; }
    [Required]
    [Range(0.1, 10000, ErrorMessage = "Distance must be between 0.1 and 10000 miles")]
    public decimal Distance { get; set; }
    [Required]
    [Range(0.01, 1000, ErrorMessage = "BasePrice must be between $0.01 and $1000")]
    public decimal BasePrice { get; set; }
}