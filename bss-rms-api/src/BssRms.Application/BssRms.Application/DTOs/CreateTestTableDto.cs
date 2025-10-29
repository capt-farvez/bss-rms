using System.ComponentModel.DataAnnotations;

namespace BssRms.Application.DTOs;

public class CreateTestTableDto
{
    [Required]
    [MaxLength(500)]
    public string TestDescription { get; set; } = string.Empty;
}
