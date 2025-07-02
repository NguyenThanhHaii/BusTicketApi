using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class AgeBasedDiscount
{
    [Key]
    public int DiscountId { get; set; }
    [Range(0, int.MaxValue)]
    public int MinAge { get; set; }
    [Range(0, int.MaxValue)]
    public int MaxAge { get; set; }
    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }
    [StringLength(200)]
    public string Description { get; set; }
}