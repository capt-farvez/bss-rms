using BssRms.Application.DTOs.Auth;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signUp")]
    public async Task<ActionResult<AuthResponseDto>> SignUp([FromBody] SignUpDto signUpDto)
    {
        try
        {
            var result = await _authService.SignUpAsync(signUpDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return ex is InvalidOperationException
                ? BadRequest(new { message = ex.Message })
                : StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("signIn")]
    public async Task<ActionResult<AuthResponseDto>> SignIn([FromBody] SignInDto signInDto)
    {
        try
        {
            var result = await _authService.SignInAsync(signInDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return ex is UnauthorizedAccessException
                ? Unauthorized(new { message = ex.Message })
                : StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return ex is UnauthorizedAccessException
                ? Unauthorized(new { message = ex.Message })
                : StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = await _authService.GetProfileAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return ex is InvalidOperationException
                ? NotFound(new { message = ex.Message })
                : StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("allUsers")]
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetAllUsers()
    {
        try
        {
            var result = await _authService.GetAllUsersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("datatable")]
    public async Task<ActionResult<DataTableResponseDto<UserListDto>>> GetUsersDatatable(
        [FromQuery] int Page = 1,
        [FromQuery] int Per_Page = 10,
        [FromQuery] string? Search = null,
        [FromQuery] string? Sort = null)
    {
        try
        {
            var result = await _authService.GetUsersDatatableAsync(Page, Per_Page, Search, Sort);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("phoneNumberExist/{phoneNumber}")]
    public async Task<ActionResult<bool>> PhoneNumberExists(string phoneNumber)
    {
        try
        {
            var exists = await _authService.PhoneNumberExistsAsync(phoneNumber);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
