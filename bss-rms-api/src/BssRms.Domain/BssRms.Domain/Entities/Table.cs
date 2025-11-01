namespace BssRms.Domain.Entities;

public class Table
{
    public int TableId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public string Image { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<EmployeeTable> EmployeeTables { get; set; } = new List<EmployeeTable>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
