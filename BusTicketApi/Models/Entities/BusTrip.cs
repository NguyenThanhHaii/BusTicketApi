using BusTicketApi.Models.Entities;
using Route = BusTicketApi.Models.Entities.Route;

public class BusTrip
{
    public int TripId { get; set; }
    public int BusId { get; set; }
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; } // Thêm nếu cần, nullable
    public int AvailableSeats { get; set; }
    public string Status { get; set; }

    public Bus Bus { get; set; }
    public Route Route { get; set; }
    public List<SeatInBusTrip> Seats { get; set; }
}