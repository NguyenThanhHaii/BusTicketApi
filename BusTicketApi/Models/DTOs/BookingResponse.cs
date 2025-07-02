using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class BookingResponse
{
    public int BookingId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime BookingDate { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalAmount { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalTax { get; set; }
    public string Status { get; set; }
    public List<BookingDetailResponse> Details { get; set; }
}

public class BookingDetailResponse
{
    public int BookingDetailId { get; set; }
    public int SeatInTripId { get; set; }
    public int CustomerId { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TicketPrice { get; set; }
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TicketTax { get; set; }
}