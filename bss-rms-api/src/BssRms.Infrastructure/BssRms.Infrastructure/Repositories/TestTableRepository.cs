using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class TestTableRepository : ITestTableRepository
{
    private readonly ApplicationDbContext _context;

    public TestTableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestTable>> GetAllAsync()
    {
        return await _context.TestTables.ToListAsync();
    }

    public async Task<TestTable?> GetByIdAsync(int id)
    {
        return await _context.TestTables.FindAsync(id);
    }

    public async Task<TestTable> CreateAsync(TestTable testTable)
    {
        _context.TestTables.Add(testTable);
        await _context.SaveChangesAsync();
        return testTable;
    }
}
