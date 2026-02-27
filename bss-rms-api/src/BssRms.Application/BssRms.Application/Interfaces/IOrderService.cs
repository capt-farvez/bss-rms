using BssRms.Application.DTOs.Order;

namespace BssRms.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDatatableItemDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDatatableItemDto?> GetByIdAsync(int id);
    Task<List<OrderSimpleDto>> GetAllAsync();
    Task<OrderDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort, int? status);
    Task<OrderDatatableItemDto> UpdateAsync(int id, CreateOrderDto dto);
    Task<OrderDatatableItemDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto);
    Task<bool> DeleteAsync(int id);
}
