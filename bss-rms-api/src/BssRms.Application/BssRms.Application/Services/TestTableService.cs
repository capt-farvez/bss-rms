using BssRms.Application.DTOs;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class TestTableService : ITestTableService
{
    private readonly ITestTableRepository _repository;

    public TestTableService(ITestTableRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TestTableDto>> GetAllAsync()
    {
        var testTables = await _repository.GetAllAsync();
        return testTables.Select(MapToDto);
    }

    public async Task<TestTableDto?> GetByIdAsync(int id)
    {
        var testTable = await _repository.GetByIdAsync(id);
        return testTable != null ? MapToDto(testTable) : null;
    }

    public async Task<TestTableDto> CreateAsync(CreateTestTableDto createDto)
    {
        var testTable = new TestTable
        {
            TestDescription = createDto.TestDescription,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(testTable);
        return MapToDto(created);
    }

    private static TestTableDto MapToDto(TestTable testTable)
    {
        return new TestTableDto
        {
            Id = testTable.Id,
            TestDescription = testTable.TestDescription,
            CreatedAt = testTable.CreatedAt
        };
    }
}
