using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User?> GetByNidAsync(string nid);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}
