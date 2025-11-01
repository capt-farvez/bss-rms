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
}
