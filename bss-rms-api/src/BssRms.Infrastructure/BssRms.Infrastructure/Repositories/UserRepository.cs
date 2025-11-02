using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Uid == userId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User?> GetByNidAsync(string nid)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Nid == nid);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByRefreshTokenAsync(string hashedRefreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int perPage, string? search, string? sort)
    {
        var query = _context.Users.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.Email.Contains(search) ||
                (u.UserName != null && u.UserName.Contains(search)) ||
                (u.FirstName != null && u.FirstName.Contains(search)) ||
                (u.LastName != null && u.LastName.Contains(search)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "email" => query.OrderBy(u => u.Email),
                "email_desc" => query.OrderByDescending(u => u.Email),
                "username" => query.OrderBy(u => u.UserName),
                "username_desc" => query.OrderByDescending(u => u.UserName),
                "firstname" => query.OrderBy(u => u.FirstName),
                "firstname_desc" => query.OrderByDescending(u => u.FirstName),
                "createdat" => query.OrderBy(u => u.CreatedAt),
                "createdat_desc" => query.OrderByDescending(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        // Apply pagination
        var users = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (users, totalCount);
    }
}
