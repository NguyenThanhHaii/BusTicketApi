namespace BusTicketApi.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CancellationResponse
{
    public int BookingId { get; set; }
    public DateTime CancellationDate { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal RefundAmount { get; set; } // $USD
    public string Status { get; set; }
}