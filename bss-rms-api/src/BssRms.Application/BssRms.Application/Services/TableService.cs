using BssRms.Application.DTOs.Table;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Application.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableDto> CreateAsync(CreateTableDto dto)
    {
        try
        {
            if (await _tableRepository.ExistsByTableNumberAsync(dto.TableNumber))
            {
                throw new InvalidOperationException($"Table number '{dto.TableNumber}' already exists.");
            }

            // Store just the filename, remove any path prefix
            var imageName = dto.Image;
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("/images/table/", "").Replace("\\images\\table\\", "");
            }

            var table = new Table
            {
                TableNumber = dto.TableNumber,
                NumberOfSeats = dto.NumberOfSeats,
                Image = imageName ?? string.Empty,
                ImageBase64 = dto.Base64 ?? string.Empty
            };

            var createdTable = await _tableRepository.CreateAsync(table);
            return MapToDto(createdTable);
        }
        catch (Exception ex)
        {
            throw ex is InvalidOperationException ? ex : new Exception($"Error creating table: {ex.Message}", ex);
        }
    }

    public async Task<TableDetailDto?> GetByIdAsync(int id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
                return null;

            var employees = table.EmployeeTables.Select(et => new TableEmployeeDto
            {
                EmployeeTableId = et.EmployeeTableId,
                EmployeeId = et.EmployeeId,
                Name = et.Employee?.User != null
                    ? $"{et.Employee.User.FirstName} {et.Employee.User.LastName}".Trim()
                    : string.Empty
            }).ToList();

            var isOccupied = table.Orders.Any(o => o.Status == 1 || o.Status == 2);

            return new TableDetailDto
            {
                Id = table.TableId,
                TableNumber = table.TableNumber,
                NumberOfSeats = table.NumberOfSeats,
                IsOccupied = isOccupied,
                Image = table.Image,
                Employees = employees
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving table: {ex.Message}", ex);
        }
    }

    public async Task<List<TableSimpleDto>> GetAllAsync()
    {
        try
        {
            var tables = await _tableRepository.GetAllAsync();
            return tables.Select(t => new TableSimpleDto
            {
                TableId = t.TableId,
                TableNumber = t.TableNumber
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving tables: {ex.Message}", ex);
        }
    }

    public async Task<TableDatatableSimpleDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var query = _tableRepository.QueryTables();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.TableNumber.Contains(search));

            // Get count before pagination (lightweight, no joins)
            var totalRecords = await query.CountAsync();
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "tablenumber" => query.OrderBy(t => t.TableNumber),
                    "-tablenumber" => query.OrderByDescending(t => t.TableNumber),
                    "numberofseats" => query.OrderBy(t => t.NumberOfSeats),
                    "-numberofseats" => query.OrderByDescending(t => t.NumberOfSeats),
                    "createdat" => query.OrderBy(t => t.CreatedAt),
                    "-createdat" => query.OrderByDescending(t => t.CreatedAt),
                    _ => query.OrderBy(t => t.CreatedAt)
                };
            }
            else
            {
                query = query.OrderBy(t => t.CreatedAt);
            }

            // Paginate and project directly into DTOs — single SQL query
            var tableDtos = await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(t => new TableDetailDto
                {
                    Id = t.TableId,
                    TableNumber = t.TableNumber,
                    NumberOfSeats = t.NumberOfSeats,
                    IsOccupied = t.Orders.Any(o => o.Status == 1 || o.Status == 2),
                    Image = t.Image,
                    Employees = t.EmployeeTables.Select(et => new TableEmployeeDto
                    {
                        EmployeeTableId = et.EmployeeTableId,
                        EmployeeId = et.EmployeeId,
                        Name = et.Employee != null && et.Employee.User != null
                            ? (et.Employee.User.FirstName ?? "") + " " + (et.Employee.User.LastName ?? "")
                            : ""
                    }).ToList()
                })
                .ToListAsync();

            return new TableDatatableSimpleDto
            {
                Data = tableDtos,
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving table datatable: {ex.Message}", ex);
        }
    }

    public async Task<TableDto> UpdateAsync(int id, UpdateTableDto dto)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {id} not found.");
            }

            if (await _tableRepository.ExistsByTableNumberAsync(dto.TableNumber, id))
            {
                throw new InvalidOperationException($"Table number '{dto.TableNumber}' already exists.");
            }

            table.TableNumber = dto.TableNumber;
            table.NumberOfSeats = dto.NumberOfSeats;

            // Only update image if new one is provided
            if (!string.IsNullOrEmpty(dto.Image))
            {
                // Store just the filename, remove any path prefix
                var imageName = dto.Image.Replace("/images/table/", "").Replace("\\images\\table\\", "");
                table.Image = imageName;
                table.ImageBase64 = dto.Base64 ?? string.Empty;
            }

            var updatedTable = await _tableRepository.UpdateAsync(table);
            return MapToDto(updatedTable);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException || ex is InvalidOperationException ? ex : new Exception($"Error updating table: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                throw new KeyNotFoundException($"Table with ID {id} not found.");
            }

            return await _tableRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting table: {ex.Message}", ex);
        }
    }

    private TableDto MapToDto(Table table)
    {
        return new TableDto
        {
            Id = table.TableId,
            TableNumber = table.TableNumber,
            NumberOfSeats = table.NumberOfSeats,
            Image = table.Image,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };
    }
}
