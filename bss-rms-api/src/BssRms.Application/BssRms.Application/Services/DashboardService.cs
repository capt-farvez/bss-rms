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

    public DashboardService(
        IOrderRepository orderRepository,
        IEmployeeRepository employeeRepository,
        IFoodRepository foodRepository,
        ITableRepository tableRepository)
    {
        _orderRepository = orderRepository;
        _employeeRepository = employeeRepository;
        _foodRepository = foodRepository;
        _tableRepository = tableRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
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
