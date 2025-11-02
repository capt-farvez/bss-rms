using BssRms.Application.DTOs.Auth;

namespace BssRms.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> SignUpAsync(SignUpDto signUpDto);
    Task<AuthResponseDto> SignInAsync(SignInDto signInDto);
    Task<ProfileDto> GetProfileAsync(Guid userId);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<IEnumerable<UserListDto>> GetAllUsersAsync();
    Task<DataTableResponseDto<UserListDto>> GetUsersDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
}
