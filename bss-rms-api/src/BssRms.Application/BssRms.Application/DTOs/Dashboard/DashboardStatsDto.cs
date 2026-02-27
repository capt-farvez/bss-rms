namespace BssRms.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalFoods { get; set; }
    public int TodaysOrders { get; set; }
    public decimal TodaysRevenue { get; set; }
    public int TotalTables { get; set; }
    public int OccupiedTables { get; set; }
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<TopSellingFoodDto> TopSellingFoods { get; set; } = new();
}

public class RecentOrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public DateTime OrderTime { get; set; }
    public string TableNumber { get; set; } = string.Empty;
}

public class TopSellingFoodDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
