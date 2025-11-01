namespace BssRms.Domain.Entities;

public class EmployeeTable
{
    public int EmployeeTableId { get; set; }
    public Guid EmployeeId { get; set; }
    public int TableId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;
    public Table Table { get; set; } = null!;
}
