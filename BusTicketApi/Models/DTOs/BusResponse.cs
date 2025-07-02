using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class BusResponse
{
    public int BusId { get; set; }
    public string BusCode { get; set; }
    public string BusNumber { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    [DisplayFormat(DataFormatString = "{0:F2}")]
    public decimal PriceMultiplier { get; set; }
    public int TotalSeats { get; set; }
}