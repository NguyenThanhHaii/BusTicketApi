namespace BusTicketApi.Models.DTOs;

public class SeatInBusTripResponse
{
    public int SeatInTripId { get; set; }
    public int TripId { get; set; }
    public string SeatNumber { get; set; }
    public bool IsAvailable { get; set; }
    public string RouteCode { get; set; } // Từ BusTrip.Route
    public string BusNumber { get; set; } // Từ BusTrip.Bus
    public DateTime DepartureTime { get; set; } // Từ BusTrip
}