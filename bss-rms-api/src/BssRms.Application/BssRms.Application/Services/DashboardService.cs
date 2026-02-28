using BssRms.Application.DTOs.Dashboard;
using BssRms.Application.Interfaces;
using BssRms.Domain.Enums;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IExpenseRepository _expenseRepository;

    public DashboardService(
        IOrderRepository orderRepository,
        IEmployeeRepository employeeRepository,
        IFoodRepository foodRepository,
        ITableRepository tableRepository,
        IExpenseRepository expenseRepository)
    {
        _orderRepository = orderRepository;
        _employeeRepository = employeeRepository;
        _foodRepository = foodRepository;
        _tableRepository = tableRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(int? month = null, int? year = null)
    {
        // General stats
        var totalOrders = await _orderRepository.GetTotalCountAsync();
        var totalRevenue = await _orderRepository.GetTotalRevenueAsync();
        var totalEmployees = await _employeeRepository.GetTotalCountAsync();
        var totalFoods = await _foodRepository.GetTotalCountAsync();
        var todaysOrders = await _orderRepository.GetTodaysOrderCountAsync();
        var todaysRevenue = await _orderRepository.GetTodaysRevenueAsync();
        var totalTables = await _tableRepository.GetTotalCountAsync();
        var occupiedTables = await _tableRepository.GetOccupiedCountAsync();

        var recentOrders = await _orderRepository.GetRecentOrdersAsync(5);
        var topSellingFoods = await _orderRepository.GetTopSellingFoodsAsync(5);

        // Sales & Revenue date ranges
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);

        var selectedMonth = month ?? now.Month;
        var selectedYear = year ?? now.Year;
        var monthStart = new DateTime(selectedYear, selectedMonth, 1);
        var monthEnd = monthStart.AddMonths(1);

        var yearStart = new DateTime(selectedYear, 1, 1);
        var yearEnd = yearStart.AddYears(1);

        // Sales (Paid orders only)
        var todaysSalesCount = await _orderRepository.GetPaidOrderCountAsync(todayStart, todayEnd);
        var todaysSalesAmount = await _orderRepository.GetPaidOrderAmountAsync(todayStart, todayEnd);
        var monthlySalesCount = await _orderRepository.GetPaidOrderCountAsync(monthStart, monthEnd);
        var monthlySalesAmount = await _orderRepository.GetPaidOrderAmountAsync(monthStart, monthEnd);
        var yearlySalesCount = await _orderRepository.GetPaidOrderCountAsync(yearStart, yearEnd);
        var yearlySalesAmount = await _orderRepository.GetPaidOrderAmountAsync(yearStart, yearEnd);
        var totalSalesCount = await _orderRepository.GetPaidOrderCountAsync(null, null);
        var totalSalesAmount = await _orderRepository.GetPaidOrderAmountAsync(null, null);

        // Expenses
        var todaysExpenses = await _expenseRepository.GetTotalExpenseAmountAsync(todayStart, todayEnd);
        var monthlyExpenses = await _expenseRepository.GetTotalExpenseAmountAsync(monthStart, monthEnd);
        var yearlyExpenses = await _expenseRepository.GetTotalExpenseAmountAsync(yearStart, yearEnd);
        var totalExpenses = await _expenseRepository.GetTotalExpenseAmountAsync(null, null);

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalEmployees = totalEmployees,
            TotalFoods = totalFoods,
            TodaysOrders = todaysOrders,
            TodaysRevenue = todaysRevenue,
            TotalTables = totalTables,
            OccupiedTables = occupiedTables,
            SalesRevenue = new SalesRevenueStatsDto
            {
                TodaysSales = todaysSalesCount,
                MonthlySales = monthlySalesCount,
                YearlySales = yearlySalesCount,
                TotalSales = totalSalesCount,
                TodaysSalesAmount = todaysSalesAmount,
                MonthlySalesAmount = monthlySalesAmount,
                YearlySalesAmount = yearlySalesAmount,
                TotalSalesAmount = totalSalesAmount,
                TodaysExpenses = todaysExpenses,
                MonthlyExpenses = monthlyExpenses,
                YearlyExpenses = yearlyExpenses,
                TotalExpenses = totalExpenses,
                TodaysRevenue = todaysSalesAmount - todaysExpenses,
                MonthlyRevenue = monthlySalesAmount - monthlyExpenses,
                YearlyRevenue = yearlySalesAmount - yearlyExpenses,
                TotalRevenue = totalSalesAmount - totalExpenses
            },
            RecentOrders = recentOrders.Select(o => new RecentOrderDto
            {
                Id = o.OrderId.ToString(),
                OrderNumber = o.OrderNumber,
                Amount = o.Amount,
                OrderStatus = o.Status switch
                {
                    (int)OrderStatus.Pending => "Pending",
                    (int)OrderStatus.Confirmed => "Confirmed",
                    (int)OrderStatus.Preparing => "Preparing",
                    (int)OrderStatus.PreparedToServe => "PreparedToServe",
                    (int)OrderStatus.Served => "Served",
                    (int)OrderStatus.Paid => "Paid",
                    _ => "Pending"
                },
                OrderTime = o.OrderDate,
                TableNumber = o.Table.TableNumber
            }).ToList(),
            TopSellingFoods = topSellingFoods.Select(f => new TopSellingFoodDto
            {
                Id = f.FoodId,
                Name = f.FoodName,
                Price = f.FoodPrice,
                Image = f.FoodImage,
                TotalQuantitySold = f.TotalQuantity,
                TotalRevenue = f.TotalRevenue
            }).ToList()
        };
    }
}
