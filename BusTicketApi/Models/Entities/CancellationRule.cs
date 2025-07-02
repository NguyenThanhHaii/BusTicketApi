using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class CancellationRule
{
    public int RuleId { get; set; }
    [Range(0, int.MaxValue)]
    public int DaysBeforeDeparture { get; set; }
    [Range(0, 100)]
    public decimal PenaltyPercentage { get; set; }
    [StringLength(200)]
    public string Description { get; set; }
}