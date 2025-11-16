using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetAllAsync();
    Task<(List<Order> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
