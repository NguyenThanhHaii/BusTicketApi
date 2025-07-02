using System.ComponentModel.DataAnnotations;
namespace BusTicketApi.Models.DTOs;
public class BusTripSearchResponse
{
    public int TripId { get; set; }
    public string BusNumber { get; set; }
    public string TypeName { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int AvailableSeats { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal BasePrice { get; set; } // Hiển thị $50.00
}