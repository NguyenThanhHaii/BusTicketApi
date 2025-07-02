namespace BusTicketApi.Models.DTOs;

public class BusTripSearchRequest
{
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public DateTime? DepartureDate { get; set; }
}