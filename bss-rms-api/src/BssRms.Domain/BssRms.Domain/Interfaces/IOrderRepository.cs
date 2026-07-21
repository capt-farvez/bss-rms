using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByIdLeanAsync(int id);
    Task<Order?> GetByIdWithItemsAsync(int id);
    Task<List<Order>> GetAllAsync();
    Task<(List<Order> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort, int? status);
    IQueryable<Order> QueryOrders();
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<List<Order>> GetRecentOrdersAsync(int count);
    Task<List<(int FoodId, string FoodName, decimal FoodPrice, string FoodImage, int TotalQuantity, decimal TotalRevenue)>> GetTopSellingFoodsAsync(int count);
    Task<int> GetTodaysOrderCountAsync();
    Task<decimal> GetTodaysRevenueAsync();
    Task<int> GetPaidOrderCountAsync(DateTime? from, DateTime? to);
    Task<decimal> GetPaidOrderAmountAsync(DateTime? from, DateTime? to);
}
