using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    [Required, StringLength(50)]
    public string Username { get; set; }
    [Required, StringLength(256)]
    public string PasswordHash { get; set; }
    [Required, StringLength(100)]
    public string Name { get; set; }
    [StringLength(100)]
    public string Email { get; set; }
    [StringLength(20)]
    public string PhoneNumber { get; set; }
    [StringLength(200)]
    public string Qualifications { get; set; }
    [Required, StringLength(20)]
    public string Role { get; set; }
    [Range(18, int.MaxValue)]
    public int? Age { get; set; }
    [StringLength(100)]
    public string Location { get; set; }
}