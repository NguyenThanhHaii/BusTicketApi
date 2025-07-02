
public class SeatInBusTrip
{
    public int SeatInTripId { get; set; }
    public int TripId { get; set; }
    public string SeatNumber { get; set; }
    public bool IsAvailable { get; set; }

    public BusTrip Trip { get; set; }
}