using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

// Models/Entities/BusType.cs
public class BusType
{
    public int TypeId { get; set; }
    [Required, StringLength(50)]
    public string TypeName { get; set; }
    [Range(1.0, double.MaxValue)]
    public decimal PriceMultiplier { get; set; }
}