using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface ITestTableRepository
{
    Task<IEnumerable<TestTable>> GetAllAsync();
    Task<TestTable?> GetByIdAsync(int id);
    Task<TestTable> CreateAsync(TestTable testTable);
}
