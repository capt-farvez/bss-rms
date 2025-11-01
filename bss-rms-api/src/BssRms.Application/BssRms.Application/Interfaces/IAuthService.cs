using BssRms.Application.DTOs.Auth;

namespace BssRms.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> SignUpAsync(SignUpDto signUpDto);
    Task<AuthResponseDto> SignInAsync(SignInDto signInDto);
    Task<ProfileDto> GetProfileAsync(Guid userId);
}
