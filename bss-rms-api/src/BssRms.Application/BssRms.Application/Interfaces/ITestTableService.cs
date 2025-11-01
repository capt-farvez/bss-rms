using BssRms.Application.DTOs.TestTable;

namespace BssRms.Application.Interfaces;

public interface ITestTableService
{
    Task<IEnumerable<TestTableDto>> GetAllAsync();
    Task<TestTableDto?> GetByIdAsync(int id);
    Task<TestTableDto> CreateAsync(CreateTestTableDto createDto);
}
 