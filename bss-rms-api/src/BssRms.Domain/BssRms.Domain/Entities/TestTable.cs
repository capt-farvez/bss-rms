namespace BssRms.Domain.Entities;

public class TestTable
{
    public int Id { get; set; }
    public string TestDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
