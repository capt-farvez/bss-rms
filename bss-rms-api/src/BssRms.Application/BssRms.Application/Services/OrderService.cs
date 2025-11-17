using BssRms.Application.DTOs.Food;
using BssRms.Application.DTOs.Order;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Enums;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEmployeeTableRepository _employeeTableRepository;

    public OrderService(IOrderRepository orderRepository, IEmployeeTableRepository employeeTableRepository)
    {
        _orderRepository = orderRepository;
        _employeeTableRepository = employeeTableRepository;
    }

    public async Task<OrderDatatableItemDto> CreateAsync(CreateOrderDto dto)
    {
        try
        {
            // Find the employees assigned to this table
            var employeeTableAssignments = await _employeeTableRepository.GetAllAsync();
            var tableEmployees = employeeTableAssignments
                .Where(et => et.TableId == dto.TableId)
                .ToList();

            // If multiple employees are assigned, use the first one, otherwise null
            var employeeId = tableEmployees.FirstOrDefault()?.EmployeeId;

            var order = new Order
            {
                TableId = dto.TableId,
                OrderNumber = dto.OrderNumber,
                Amount = dto.Amount,
                PhoneNumber = dto.PhoneNumber,
                OrderedById = employeeId,
                OrderTakenById = employeeId,
                OrderItems = dto.Items.Select(item => new OrderItem
                {
                    FoodId = item.FoodId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Reload with all relationships
            var orderWithRelations = await _orderRepository.GetByIdAsync(createdOrder.OrderId);
            return MapToDatatableItemDto(orderWithRelations!);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating order: {ex.Message}", ex);
        }
    }

    public async Task<OrderDatatableItemDto?> GetByIdAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : MapToDatatableItemDto(order);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving order: {ex.Message}", ex);
        }
    }

    public async Task<List<OrderSimpleDto>> GetAllAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderSimpleDto
            {
                OrderId = o.OrderId.ToString(),
                OrderNumber = o.OrderNumber
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving orders: {ex.Message}", ex);
        }
    }

    public async Task<OrderDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var (data, totalRecords) = await _orderRepository.GetDatatableAsync(page, perPage, search, sort);
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            var orderDtos = data.Select(MapToDatatableItemDto).ToList();

            return new OrderDatatableDto
            {
                Data = orderDtos,
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving order datatable: {ex.Message}", ex);
        }
    }

    public async Task<OrderDatatableItemDto> UpdateAsync(int id, CreateOrderDto dto)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            order.TableId = dto.TableId;
            order.Amount = dto.Amount;
            order.PhoneNumber = dto.PhoneNumber;

            // Update order items - clear existing items and add new ones
            order.OrderItems.Clear();
            order.OrderItems = dto.Items.Select(item => new OrderItem
            {
                FoodId = item.FoodId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList();

            var updatedOrder = await _orderRepository.UpdateAsync(order);

            // Reload with all relationships
            var orderWithRelations = await _orderRepository.GetByIdAsync(updatedOrder.OrderId);
            return MapToDatatableItemDto(orderWithRelations!);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error updating order: {ex.Message}", ex);
        }
    }

    public async Task<OrderDatatableItemDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            int statusValue = dto.Status.ToLower() switch
            {
                "0" or "pending" => (int)OrderStatus.Pending,
                "1" or "confirmed" => (int)OrderStatus.Confirmed,
                "2" or "preparing" => (int)OrderStatus.Preparing,
                "3" or "preparedtoserve" or "prepared to serve" => (int)OrderStatus.PreparedToServe,
                "4" or "served" => (int)OrderStatus.Served,
                "5" or "paid" => (int)OrderStatus.Paid,
                _ => (int)OrderStatus.Pending // Default to Pending
            };

            order.Status = statusValue;

            var updatedOrder = await _orderRepository.UpdateAsync(order);

            // Reload with all relationships
            var orderWithRelations = await _orderRepository.GetByIdAsync(updatedOrder.OrderId);
            return MapToDatatableItemDto(orderWithRelations!);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error updating order status: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var exists = await _orderRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            return await _orderRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting order: {ex.Message}", ex);
        }
    }

    private OrderDatatableItemDto MapToDatatableItemDto(Order order)
    {
        // Map order status to string - must match Angular app expectations
        string orderStatus = order.Status switch
        {
            (int)OrderStatus.Pending => "Pending",
            (int)OrderStatus.Confirmed => "Confirmed",
            (int)OrderStatus.Preparing => "Preparing",
            (int)OrderStatus.PreparedToServe => "PreparedToServe",
            (int)OrderStatus.Served => "Served",
            (int)OrderStatus.Paid => "Paid",
            _ => "Pending"
        };

        return new OrderDatatableItemDto
        {
            Id = order.OrderId.ToString(),
            OrderNumber = order.OrderNumber,
            Amount = order.Amount,
            OrderStatus = orderStatus,
            OrderTime = order.OrderDate,
            Table = new OrderTableInfoDto
            {
                TableId = order.Table.TableId,
                TableNumber = order.Table.TableNumber
            },
            OrderedBy = MapEmployeeToUserInfoDto(order.OrderedBy),
            OrderTakenBy = MapEmployeeToUserInfoDto(order.OrderTakenBy),
            OrderItems = order.OrderItems.Select(oi => new OrderItemDetailDto
            {
                Id = oi.OrderItemId.ToString(),
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                Food = MapToFoodDto(oi.Food)
            }).ToList()
        };
    }

    private OrderUserInfoDto MapEmployeeToUserInfoDto(Employee? employee)
    {
        if (employee == null || employee.User == null)
        {
            return new OrderUserInfoDto
            {
                Id = Guid.Empty,
                UserName = null,
                Email = null,
                FullName = "Unknown",
                PhoneNumber = null,
                FirstName = "Unknown",
                LastName = null,
                Image = null
            };
        }

        var user = employee.User;
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            fullName = user.UserName ?? "Employee";
        }

        return new OrderUserInfoDto
        {
            Id = user.Uid,
            UserName = user.UserName,
            Email = user.Email,
            FullName = fullName,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName ?? "Employee",
            LastName = user.LastName,
            Image = user.Image
        };
    }

    private FoodDatatableItemDto MapToFoodDto(Food food)
    {
        // Calculate discount price based on discount type
        decimal discountPrice = food.Price;
        string discountTypeStr = "None";

        switch (food.DiscountType)
        {
            case 1: // Percentage
                discountTypeStr = "Percentage";
                discountPrice = food.Price - (food.Price * food.Discount / 100);
                break;
            case 2: // Flat/Fixed
                discountTypeStr = "Flat";
                discountPrice = food.Price - food.Discount;
                break;
            default: // None
                discountTypeStr = "None";
                break;
        }

        return new FoodDatatableItemDto
        {
            Id = food.FoodId,
            Name = food.Name,
            Description = food.Description,
            Price = food.Price,
            DiscountType = discountTypeStr,
            Discount = food.Discount,
            DiscountPrice = discountPrice,
            Image = food.Image
        };
    }
}
