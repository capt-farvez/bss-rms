using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Employee;

public class UpdateEmployeeDto
{
    [JsonPropertyName("designation")]
    public string? Designation { get; set; }

    [JsonPropertyName("joinDate")]
    public DateTime? JoinDate { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("middleName")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("fatherName")]
    public string? FatherName { get; set; }

    [JsonPropertyName("motherName")]
    public string? MotherName { get; set; }

    [JsonPropertyName("spouseName")]
    public string? SpouseName { get; set; }

    [JsonPropertyName("dob")]
    public DateTime? Dob { get; set; }

    [JsonPropertyName("nid")]
    public string? Nid { get; set; }

    [JsonPropertyName("genderId")]
    public int? GenderId { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("base64")]
    public string? Base64 { get; set; }
}
