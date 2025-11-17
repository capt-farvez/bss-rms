namespace BssRms.Application.DTOs.Employee;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public string? SpouseName { get; set; }
    public DateTime? Dob { get; set; }
    public string? Nid { get; set; }
    public int GenderId { get; set; }
    public string? Image { get; set; }
}
