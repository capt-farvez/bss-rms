namespace BssRms.Application.DTOs.Table;

public class CreateTableDto
{
    public string TableNumber { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
}
