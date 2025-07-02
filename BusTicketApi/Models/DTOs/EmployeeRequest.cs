using System.ComponentModel.DataAnnotations;

namespace BusTicketApi.Models.DTOs;

public class EmployeeRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string Name { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    [Required]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; }
    [StringLength(200, ErrorMessage = "Qualifications must not exceed 200 characters")]
    public string Qualifications { get; set; }
    [Required]
    [RegularExpression("^(Admin|Employee)$", ErrorMessage = "Role must be 'Admin' or 'Employee'")]
    public string Role { get; set; }
    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
    public int? Age { get; set; }
    [StringLength(100, ErrorMessage = "Location must not exceed 100 characters")]
    public string Location { get; set; }
}