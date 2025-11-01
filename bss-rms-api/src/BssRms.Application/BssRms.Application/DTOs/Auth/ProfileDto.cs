namespace BssRms.Application.DTOs.Auth;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
}
