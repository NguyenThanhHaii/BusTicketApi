namespace BusTicketApi.Models.DTOs;

public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Qualifications { get; set; }
    public string? Role { get; set; }
    public int? Age { get; set; }
    public string? Location { get; set; }
}