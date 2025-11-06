namespace BssRms.Application.DTOs.Table;

public class TableListDto
{
    public int Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
}
