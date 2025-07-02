using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    [StringLength(100)]
    public string Email { get; set; }
    [StringLength(20)]
    public string PhoneNumber { get; set; }
}