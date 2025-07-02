using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class ReportResponse
{
    public string? Period { get; set; } // e.g., "2025-07", "2025-07-02"
    public int TotalTickets { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalRevenue { get; set; } // TotalAmount (USD)
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalTax { get; set; } // TotalTax (USD)
    public int CancelledTickets { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalRefund { get; set; } // RefundAmount (USD)
}