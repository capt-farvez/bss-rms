using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BssRms.Application.DTOs.Auth;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BssRms.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> SignUpAsync(SignUpDto signUpDto)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(signUpDto.Email))
                throw new InvalidOperationException("Email is required");
            if (string.IsNullOrEmpty(signUpDto.PhoneNumber))
                throw new InvalidOperationException("Phone number is required");
            if (string.IsNullOrEmpty(signUpDto.Password))
                throw new InvalidOperationException("Password is required");

            // Validate password strength
            if (!ValidatePasswordStrength(signUpDto.Password))
                throw new InvalidOperationException("Password must be at least 8 characters long and include uppercase, lowercase, number, and special character");

            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(signUpDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var existingPhone = await _userRepository.GetByPhoneNumberAsync(signUpDto.PhoneNumber);
            if (existingPhone != null)
            {
                throw new InvalidOperationException("User with this phone number already exists");
            }

            if (!string.IsNullOrEmpty(signUpDto.Nid))
            {
                var existingNid = await _userRepository.GetByNidAsync(signUpDto.Nid);
                if (existingNid != null)
                {
                    throw new InvalidOperationException("User with this NID already exists");
                }
            }

            if (!string.IsNullOrEmpty(signUpDto.UserName))
            {
                var existingUserName = await _userRepository.GetByUserNameAsync(signUpDto.UserName);
                if (existingUserName != null)
                {
                    throw new InvalidOperationException("User with this username already exists");
                }
            }

            // Hash password
            var passwordHash = HashPassword(signUpDto.Password);

            // Create new user
            var user = new User
            {
                Uid = Guid.NewGuid(),
                Email = signUpDto.Email,
                PhoneNumber = signUpDto.PhoneNumber,
                Password = passwordHash,
                FirstName = signUpDto.FirstName ?? string.Empty,
                MiddleName = signUpDto.MiddleName ?? string.Empty,
                LastName = signUpDto.LastName ?? string.Empty,
                FatherName = signUpDto.FatherName ?? string.Empty,
                MotherName = signUpDto.MotherName ?? string.Empty,
                SpouseName = signUpDto.SpouseName ?? string.Empty,
                Nid = signUpDto.Nid ?? string.Empty,
                Dob = signUpDto.Dob ?? DateTime.MinValue,
                GenderId = signUpDto.GenderId ?? 0,
                UserName = signUpDto.UserName,
                Image = signUpDto.Image ?? string.Empty,
                ImageBase64 = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var hashedRefreshToken = HashRefreshToken(refreshToken);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = hashedRefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

            await _userRepository.CreateAsync(user);

            var fullName = string.Join(" ", new[] { user.FirstName, user.MiddleName, user.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime,
                User = new ProfileDto
                {
                    Id = user.Uid,
                    FullName = fullName,
                    Email = user.Email,
                    Image = string.IsNullOrEmpty(user.Image) ? null : user.Image,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber
                }
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred during user registration", ex);
        }
    }

    public async Task<AuthResponseDto> SignInAsync(SignInDto signInDto)
    {
        try
        {
            if (string.IsNullOrEmpty(signInDto.UserName))
                throw new UnauthorizedAccessException("Username is required");
            if (string.IsNullOrEmpty(signInDto.Password))
                throw new UnauthorizedAccessException("Password is required");

            // Find user by username or email
            User? user = null;

            // Try to find by username first
            if (signInDto.UserName.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(signInDto.UserName);
            }
            else
            {
                user = await _userRepository.GetByUserNameAsync(signInDto.UserName);
            }

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Verify password
            if (!VerifyPassword(signInDto.Password, user.Password ?? string.Empty))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var hashedRefreshToken = HashRefreshToken(refreshToken);
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            // Update refresh token (store hashed version)
            user.RefreshToken = hashedRefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var fullName = string.Join(" ", new[] { user.FirstName, user.MiddleName, user.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime,
                User = new ProfileDto
                {
                    Id = user.Uid,
                    FullName = fullName,
                    Email = user.Email,
                    Image = string.IsNullOrEmpty(user.Image) ? null : user.Image,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber
                }
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred during sign in", ex);
        }
    }

    public async Task<ProfileDto> GetProfileAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var fullName = string.Join(" ", new[] { user.FirstName, user.MiddleName, user.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            return new ProfileDto
            {
                Id = user.Uid,
                FullName = fullName,
                Email = user.Email,
                Image = string.IsNullOrEmpty(user.Image) ? null : user.Image,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving user profile", ex);
        }
    }

    // ------------------------- Password Validation -------------------------------//
    private bool ValidatePasswordStrength(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }

            bool hasUppercase = password.Any(char.IsUpper);
            bool hasLowercase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUppercase && hasLowercase && hasDigit && hasSpecialChar;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to validate password strength", ex);
        }
    }

    // ------------------------- Password Hashing -------------------------------//
    private string HashPassword(string InputPassword)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(InputPassword));
            return Convert.ToBase64String(hashedBytes);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to hash password", ex);
        }
    }

    private bool VerifyPassword(string InputPassword, string StoredHashedPassword)
    {
        try
        {
            var hashOfInput = HashPassword(InputPassword);
            return hashOfInput == StoredHashedPassword;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to verify password", ex);
        }
    }

    // ------------------------- JWT -------------------------------//
    private string GenerateAccessToken(User user)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Uid.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Uid.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to generate access token", ex);
        }
    }

    private int GetAccessTokenExpirationMinutes()
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = jwtSettings["ExpirationMinutes"];
            return int.TryParse(expirationMinutes, out var minutes) ? minutes : 60;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get token expiration time", ex);
        }
    }

    private string GenerateRefreshToken()
    {
        try
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to generate refresh token", ex);
        }
    }
    private string HashRefreshToken(string refreshToken)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(hashBytes);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to hashed refresh token", ex);
        }
    }
}
