namespace BusTicketApi.Models.Entities;


public class Bus
{
    public int BusId { get; set; }
    public string BusCode { get; set; }
    public string BusNumber { get; set; }
    public int TypeId { get; set; }
    public int TotalSeats { get; set; }

    public BusType Type { get; set; }
    public List<BusTrip> Trips { get; set; } // Thêm quan hệ
}