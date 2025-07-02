using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class RouteStop
{
    public int RouteStopId { get; set; }
    public int RouteId { get; set; }
    [Required, StringLength(100)]
    public string StopLocation { get; set; }
    [Range(1, int.MaxValue)]
    public int StopOrder { get; set; }
    public Route Route { get; set; }
}