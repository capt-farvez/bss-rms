using BssRms.Domain.Enums;

namespace BssRms.Domain.Entities;

public class User
{
    public Guid Uid { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string SpouseName { get; set; } = string.Empty;
    public string Nid { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public int GenderId { get; set; }
    public string Image { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // One-to-one relationship with Employee
    public Employee? Employee { get; set; }
}
