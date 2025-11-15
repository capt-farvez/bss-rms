namespace BssRms.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int TableId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public int Status { get; set; }
    public Guid? OrderedById { get; set; }
    public Guid? OrderTakenById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Table Table { get; set; } = null!;
    public Employee? OrderedBy { get; set; }
    public Employee? OrderTakenBy { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
