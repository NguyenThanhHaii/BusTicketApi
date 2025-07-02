using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;
public class Route
{
    public int RouteId { get; set; }
    [Required, StringLength(20)]
    public string RouteCode { get; set; }
    [Required, StringLength(100)]
    public string StartLocation { get; set; }
    [Required, StringLength(100)]
    public string EndLocation { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Distance { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal BasePrice { get; set; }
}