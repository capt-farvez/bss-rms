namespace BssRms.Application.DTOs.Auth;

public class SignUpDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? SpouseName { get; set; }
    public string? Nid { get; set; }
    public DateTime? Dob { get; set; }
    public int? GenderId { get; set; }
    public string? Image { get; set; }
}
