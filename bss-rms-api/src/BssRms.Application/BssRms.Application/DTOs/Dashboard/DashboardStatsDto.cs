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
    public SalesRevenueStatsDto SalesRevenue { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<TopSellingFoodDto> TopSellingFoods { get; set; } = new();
}

public class SalesRevenueStatsDto
{
    public int TodaysSales { get; set; }
    public int MonthlySales { get; set; }
    public int YearlySales { get; set; }
    public int TotalSales { get; set; }

    public decimal TodaysSalesAmount { get; set; }
    public decimal MonthlySalesAmount { get; set; }
    public decimal YearlySalesAmount { get; set; }
    public decimal TotalSalesAmount { get; set; }

    public decimal TodaysExpenses { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal YearlyExpenses { get; set; }
    public decimal TotalExpenses { get; set; }

    public decimal TodaysRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal YearlyRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
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
