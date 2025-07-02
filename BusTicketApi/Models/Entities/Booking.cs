using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime BookingDate { get; set; }
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    [Range(0, double.MaxValue)]
    public decimal TotalTax { get; set; }
    [Required, StringLength(20)]
    public string Status { get; set; }
    public DateTime? CancellationDate { get; set; }
    [Range(0, double.MaxValue)]
    public decimal? RefundAmount { get; set; }
    public Employee Employee { get; set; }
    
    public List<BookingDetail> Details { get; set; }
}

