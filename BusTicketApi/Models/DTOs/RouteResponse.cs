using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class RouteResponse
{
    public int RouteId { get; set; }
    public string RouteCode { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public decimal Distance { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal BasePrice { get; set; }
}