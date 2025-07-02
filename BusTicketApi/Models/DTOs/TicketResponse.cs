using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class TicketResponse
{
    public int BookingId { get; set; }
    public string CustomerName { get; set; }
    public DateTime CustomerDateOfBirth { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhoneNumber { get; set; }
    public string Route { get; set; }
    public string BusNumber { get; set; }
    public string SeatNumber { get; set; }
    public DateTime DepartureTime { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TicketPrice { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TicketTax { get; set; }
    public DateTime BookingDate { get; set; }
    public string Status { get; set; }
}