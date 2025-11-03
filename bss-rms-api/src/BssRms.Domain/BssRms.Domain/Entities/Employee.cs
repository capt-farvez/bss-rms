namespace BssRms.Domain.Entities;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public Guid UserId { get; set; }
    public string Designation { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public decimal AmountSold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<EmployeeTable> EmployeeTables { get; set; } = new List<EmployeeTable>();
}
