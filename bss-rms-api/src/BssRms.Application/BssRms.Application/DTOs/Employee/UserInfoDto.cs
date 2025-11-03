namespace BssRms.Application.DTOs.Employee;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Image { get; set; }
}
