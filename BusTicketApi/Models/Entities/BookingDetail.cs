using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class BookingDetail
{
    public int BookingDetailId { get; set; }
    public int BookingId { get; set; }
    public int SeatInTripId { get; set; }
    public int CustomerId { get; set; }
    [Range(0, double.MaxValue)]
    public decimal TicketPrice { get; set; }
    [Range(0, double.MaxValue)]
    public decimal TicketTax { get; set; }
    public Booking Booking { get; set; }
    public SeatInBusTrip SeatInTrip { get; set; }
    public Customer Customer { get; set; }
}