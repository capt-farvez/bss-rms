using BssRms.Application.DTOs.EmployeeTable;
using BssRms.Application.Interfaces;
using BssRms.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Application.Services;

public class EmployeeTableService : IEmployeeTableService
{
    private readonly IEmployeeTableRepository _employeeTableRepository;

    public EmployeeTableService(IEmployeeTableRepository employeeTableRepository)
    {
        _employeeTableRepository = employeeTableRepository;
    }

    public async Task<EmployeeTableDto> CreateAsync(CreateEmployeeTableDto dto)
    {
        try
        {
            if (await _employeeTableRepository.ExistsAsync(dto.EmployeeId, dto.TableId))
            {
                throw new InvalidOperationException($"Employee is already assigned to this table.");
            }

            var employeeTable = new Domain.Entities.EmployeeTable
            {
                EmployeeId = dto.EmployeeId,
                TableId = dto.TableId
            };

            var createdEmployeeTable = await _employeeTableRepository.CreateAsync(employeeTable);
            var result = await _employeeTableRepository.GetByIdAsync(createdEmployeeTable.EmployeeTableId);

            return new EmployeeTableDto
            {
                Id = result!.EmployeeTableId,
                Employee = new EmployeeTableEmployeeDto
                {
                    EmployeeId = result.EmployeeId,
                    Name = result.Employee?.User != null
                        ? $"{result.Employee.User.FirstName} {result.Employee.User.LastName}".Trim()
                        : string.Empty
                },
                Table = new EmployeeTableTableDto
                {
                    TableId = result.TableId,
                    TableNumber = result.Table?.TableNumber ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            throw ex is InvalidOperationException ? ex : new Exception($"Error creating employee table assignment: {ex.Message}", ex);
        }
    }

    public async Task<string> CreateRangeAsync(List<CreateEmployeeTableDto> dtos)
    {
        try
        {
            var employeeTablesToCreate = new List<Domain.Entities.EmployeeTable>();

            foreach (var dto in dtos)
            {
                if (!await _employeeTableRepository.ExistsAsync(dto.EmployeeId, dto.TableId))
                {
                    employeeTablesToCreate.Add(new Domain.Entities.EmployeeTable
                    {
                        EmployeeId = dto.EmployeeId,
                        TableId = dto.TableId
                    });
                }
            }

            if (employeeTablesToCreate.Count == 0)
            {
                return "All provided employees are already assigned to this table.";
            }

            await _employeeTableRepository.CreateRangeAsync(employeeTablesToCreate);

            return $"Successfully assigned {employeeTablesToCreate.Count} employee(s) to the table.";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating employee table assignments: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeTableDto?> GetByIdAsync(int id)
    {
        try
        {
            var employeeTable = await _employeeTableRepository.GetByIdAsync(id);
            if (employeeTable == null)
                return null;

            return new EmployeeTableDto
            {
                Id = employeeTable.EmployeeTableId,
                Employee = new EmployeeTableEmployeeDto
                {
                    EmployeeId = employeeTable.EmployeeId,
                    Name = employeeTable.Employee?.User != null
                        ? $"{employeeTable.Employee.User.FirstName} {employeeTable.Employee.User.LastName}".Trim()
                        : string.Empty
                },
                Table = new EmployeeTableTableDto
                {
                    TableId = employeeTable.TableId,
                    TableNumber = employeeTable.Table?.TableNumber ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee table: {ex.Message}", ex);
        }
    }

    public async Task<List<EmployeeTableListDto>> GetAllAsync()
    {
        try
        {
            var employeeTables = await _employeeTableRepository.GetAllAsync();
            return employeeTables.Select(et => new EmployeeTableListDto
            {
                EmployeeTableId = et.EmployeeTableId,
                Employee = new EmployeeTableEmployeeDto
                {
                    EmployeeId = et.EmployeeId,
                    Name = et.Employee?.User != null
                        ? $"{et.Employee.User.FirstName} {et.Employee.User.LastName}".Trim()
                        : string.Empty
                },
                Table = new EmployeeTableTableDto
                {
                    TableId = et.TableId,
                    TableNumber = et.Table?.TableNumber ?? string.Empty
                }
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee tables: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeTableDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var query = _employeeTableRepository.QueryEmployeeTables();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(et =>
                    et.Employee.User.FirstName.Contains(search) ||
                    et.Employee.User.LastName.Contains(search) ||
                    et.Table.TableNumber.Contains(search));
            }

            // Get count before pagination
            var totalRecords = await query.CountAsync();
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = sort.ToLower() switch
                {
                    "employeename" => query.OrderBy(et => et.Employee.User.FirstName),
                    "-employeename" => query.OrderByDescending(et => et.Employee.User.FirstName),
                    "tablenumber" => query.OrderBy(et => et.Table.TableNumber),
                    "-tablenumber" => query.OrderByDescending(et => et.Table.TableNumber),
                    "createdat" => query.OrderBy(et => et.CreatedAt),
                    "-createdat" => query.OrderByDescending(et => et.CreatedAt),
                    _ => query.OrderBy(et => et.CreatedAt)
                };
            }
            else
            {
                query = query.OrderBy(et => et.CreatedAt);
            }

            // Paginate and project directly into DTOs — single SQL query
            var employeeTableDtos = await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(et => new EmployeeTableDto
                {
                    Id = et.EmployeeTableId,
                    Employee = new EmployeeTableEmployeeDto
                    {
                        EmployeeId = et.EmployeeId,
                        Name = et.Employee != null && et.Employee.User != null
                            ? (et.Employee.User.FirstName ?? "") + " " + (et.Employee.User.LastName ?? "")
                            : ""
                    },
                    Table = new EmployeeTableTableDto
                    {
                        TableId = et.TableId,
                        TableNumber = et.Table != null ? et.Table.TableNumber : ""
                    }
                })
                .ToListAsync();

            return new EmployeeTableDatatableDto
            {
                Data = employeeTableDtos,
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee table datatable: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeTableDto> UpdateAsync(int id, UpdateEmployeeTableDto dto)
    {
        try
        {
            var employeeTable = await _employeeTableRepository.GetByIdAsync(id);
            if (employeeTable == null)
            {
                throw new KeyNotFoundException($"EmployeeTable with ID {id} not found.");
            }

            if (await _employeeTableRepository.ExistsAsync(dto.EmployeeId, dto.TableId, id))
            {
                throw new InvalidOperationException($"Employee is already assigned to this table.");
            }

            employeeTable.EmployeeId = dto.EmployeeId;
            employeeTable.TableId = dto.TableId;

            var updatedEmployeeTable = await _employeeTableRepository.UpdateAsync(employeeTable);
            var result = await _employeeTableRepository.GetByIdAsync(updatedEmployeeTable.EmployeeTableId);

            return new EmployeeTableDto
            {
                Id = result!.EmployeeTableId,
                Employee = new EmployeeTableEmployeeDto
                {
                    EmployeeId = result.EmployeeId,
                    Name = result.Employee?.User != null
                        ? $"{result.Employee.User.FirstName} {result.Employee.User.LastName}".Trim()
                        : string.Empty
                },
                Table = new EmployeeTableTableDto
                {
                    TableId = result.TableId,
                    TableNumber = result.Table?.TableNumber ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException || ex is InvalidOperationException ? ex : new Exception($"Error updating employee table assignment: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var employeeTable = await _employeeTableRepository.GetByIdAsync(id);
            if (employeeTable == null)
            {
                throw new KeyNotFoundException($"EmployeeTable with ID {id} not found.");
            }

            return await _employeeTableRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting employee table assignment: {ex.Message}", ex);
        }
    }
}
