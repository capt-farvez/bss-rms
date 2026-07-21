using BssRms.Application.DTOs.Food;
using BssRms.Application.DTOs.Order;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Enums;
using BssRms.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            // If multiple employees are assigned, use the latest assignment, otherwise null
            var employeeId = await _employeeTableRepository.QueryEmployeeTables()
                .Where(et => et.TableId == dto.TableId)
                .OrderByDescending(et => et.EmployeeTableId)
                .Select(et => (Guid?)et.EmployeeId)
                .FirstOrDefaultAsync();

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

            return (await GetDatatableItemAsync(createdOrder.OrderId))!;
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
            return await GetDatatableItemAsync(id);
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

    public async Task<OrderDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort, int? status)
    {
        try
        {
            var query = _orderRepository.QueryOrders();

            // Apply filters
            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o =>
                    o.OrderNumber.Contains(search) ||
                    o.Table.TableNumber.Contains(search) ||
                    (o.PhoneNumber != null && o.PhoneNumber.Contains(search)));

            // Get count before pagination (lightweight, no joins)
            var totalRecords = await query.CountAsync();
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "ordernumber" => query.OrderBy(o => o.OrderNumber),
                    "-ordernumber" => query.OrderByDescending(o => o.OrderNumber),
                    "amount" => query.OrderBy(o => o.Amount),
                    "-amount" => query.OrderByDescending(o => o.Amount),
                    "orderdate" => query.OrderBy(o => o.OrderDate),
                    "-orderdate" => query.OrderByDescending(o => o.OrderDate),
                    "createdat" => query.OrderBy(o => o.CreatedAt),
                    "-createdat" => query.OrderByDescending(o => o.CreatedAt),
                    "status" => query.OrderBy(o => o.Status),
                    "-status" => query.OrderByDescending(o => o.Status),
                    _ => query.OrderBy(o => o.CreatedAt)
                };
            }
            else
            {
                query = query.OrderBy(o => o.CreatedAt);
            }

            // Paginate and project directly into DTOs — single SQL query
            var orderDtos = await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(DatatableItemProjection)
                .ToListAsync();

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
            var order = await _orderRepository.GetByIdWithItemsAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            order.TableId = dto.TableId;
            order.Amount = dto.Amount;
            order.PhoneNumber = dto.PhoneNumber;

            // Mutate the tracked collection in place: reassigning it would detach the
            // old items from change tracking and leave their rows behind in the DB.
            order.OrderItems.Clear();
            foreach (var item in dto.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    FoodId = item.FoodId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                });
            }

            var updatedOrder = await _orderRepository.UpdateAsync(order);

            return (await GetDatatableItemAsync(updatedOrder.OrderId))!;
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
            var order = await _orderRepository.GetByIdLeanAsync(id);
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

            return (await GetDatatableItemAsync(updatedOrder.OrderId))!;
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

    // Shared SQL projection for order responses. EF translates this to a column-level
    // SELECT, so the large ImageBase64 payloads on Table/User/Food never leave the
    // database — only the image filenames do.
    private static readonly Expression<Func<Order, OrderDatatableItemDto>> DatatableItemProjection = o => new OrderDatatableItemDto
    {
        Id = o.OrderId.ToString(),
        OrderNumber = o.OrderNumber,
        Amount = o.Amount,
        OrderStatus = o.Status == (int)OrderStatus.Pending ? "Pending"
            : o.Status == (int)OrderStatus.Confirmed ? "Confirmed"
            : o.Status == (int)OrderStatus.Preparing ? "Preparing"
            : o.Status == (int)OrderStatus.PreparedToServe ? "PreparedToServe"
            : o.Status == (int)OrderStatus.Served ? "Served"
            : o.Status == (int)OrderStatus.Paid ? "Paid"
            : "Pending",
        OrderTime = o.OrderDate,
        Table = new OrderTableInfoDto
        {
            TableId = o.Table.TableId,
            TableNumber = o.Table.TableNumber
        },
        OrderedBy = o.OrderedBy != null && o.OrderedBy.User != null
            ? new OrderUserInfoDto
            {
                Id = o.OrderedBy.User.Uid,
                UserName = o.OrderedBy.User.UserName,
                Email = o.OrderedBy.User.Email,
                FullName = (o.OrderedBy.User.FirstName ?? "") + " " + (o.OrderedBy.User.LastName ?? ""),
                PhoneNumber = o.OrderedBy.User.PhoneNumber,
                FirstName = o.OrderedBy.User.FirstName ?? "Employee",
                LastName = o.OrderedBy.User.LastName,
                Image = o.OrderedBy.User.Image
            }
            : new OrderUserInfoDto
            {
                Id = Guid.Empty,
                UserName = null,
                Email = null,
                FullName = "Unknown",
                PhoneNumber = null,
                FirstName = "Unknown",
                LastName = null,
                Image = null
            },
        OrderTakenBy = o.OrderTakenBy != null && o.OrderTakenBy.User != null
            ? new OrderUserInfoDto
            {
                Id = o.OrderTakenBy.User.Uid,
                UserName = o.OrderTakenBy.User.UserName,
                Email = o.OrderTakenBy.User.Email,
                FullName = (o.OrderTakenBy.User.FirstName ?? "") + " " + (o.OrderTakenBy.User.LastName ?? ""),
                PhoneNumber = o.OrderTakenBy.User.PhoneNumber,
                FirstName = o.OrderTakenBy.User.FirstName ?? "Employee",
                LastName = o.OrderTakenBy.User.LastName,
                Image = o.OrderTakenBy.User.Image
            }
            : new OrderUserInfoDto
            {
                Id = Guid.Empty,
                UserName = null,
                Email = null,
                FullName = "Unknown",
                PhoneNumber = null,
                FirstName = "Unknown",
                LastName = null,
                Image = null
            },
        OrderItems = o.OrderItems.Select(oi => new OrderItemDetailDto
        {
            Id = oi.OrderItemId.ToString(),
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            TotalPrice = oi.TotalPrice,
            Food = new FoodDatatableItemDto
            {
                Id = oi.Food.FoodId,
                Name = oi.Food.Name,
                Description = oi.Food.Description,
                Price = oi.Food.Price,
                DiscountType = oi.Food.DiscountType == 1 ? "Percentage"
                    : oi.Food.DiscountType == 2 ? "Flat"
                    : "None",
                Discount = oi.Food.Discount,
                DiscountPrice = oi.Food.DiscountType == 1
                    ? oi.Food.Price - (oi.Food.Price * oi.Food.Discount / 100)
                    : oi.Food.DiscountType == 2
                        ? oi.Food.Price - oi.Food.Discount
                        : oi.Food.Price,
                Image = oi.Food.Image
            }
        }).ToList()
    };

    private Task<OrderDatatableItemDto?> GetDatatableItemAsync(int orderId)
    {
        return _orderRepository.QueryOrders()
            .Where(o => o.OrderId == orderId)
            .Select(DatatableItemProjection)
            .FirstOrDefaultAsync();
    }
}
