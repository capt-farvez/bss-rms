using BssRms.Application.DTOs.Table;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;

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

            var table = new Table
            {
                TableNumber = dto.TableNumber,
                NumberOfSeats = dto.NumberOfSeats,
                Image = dto.Image,
                ImageBase64 = dto.Base64
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
            var (data, totalRecords) = await _tableRepository.GetDatatableAsync(page, perPage, search, sort);
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            var tableDtos = data.Select(table =>
            {
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
            }).ToList();

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
            table.Image = dto.Image;
            table.ImageBase64 = dto.Base64;

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
