using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BssRms.Infrastructure.Data;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("images")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{*imageName}")]
    public async Task<IActionResult> GetUserImage(string imageName)
    {
        try
        {
            // URL decode the image name and trim any leading slashes
            var decodedImageName = Uri.UnescapeDataString(imageName).TrimStart('/');

            // The database stores just the filename
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Image == decodedImageName);

            if (user == null || string.IsNullOrEmpty(user.ImageBase64))
            {
                return NotFound(new { message = $"Image not found: {decodedImageName}" });
            }

            // Parse the base64 string to get content type and data
            var base64Data = user.ImageBase64;

            // Check if it includes the data URI prefix (e.g., "data:image/jpeg;base64,")
            if (base64Data.StartsWith("data:"))
            {
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex > 0)
                {
                    var metaData = base64Data.Substring(0, commaIndex);
                    base64Data = base64Data.Substring(commaIndex + 1);

                    // Extract content type from metadata
                    var contentType = "image/jpeg"; // Default
                    if (metaData.Contains("image/png"))
                        contentType = "image/png";
                    else if (metaData.Contains("image/gif"))
                        contentType = "image/gif";
                    else if (metaData.Contains("image/webp"))
                        contentType = "image/webp";
                    else if (metaData.Contains("image/bmp"))
                        contentType = "image/bmp";

                    var imageBytes = Convert.FromBase64String(base64Data);
                    return File(imageBytes, contentType);
                }
            }

            // If no data URI prefix, assume it's plain base64
            var bytes = Convert.FromBase64String(base64Data);
            return File(bytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error retrieving image: {ex.Message}" });
        }
    }

    [HttpGet("table/{*imageName}")]
    public async Task<IActionResult> GetTableImage(string imageName)
    {
        try
        {
            // URL decode the image name and trim any leading slashes
            var decodedImageName = Uri.UnescapeDataString(imageName).TrimStart('/');

            // The database stores just the filename
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Image == decodedImageName);

            if (table == null || string.IsNullOrEmpty(table.ImageBase64))
            {
                return NotFound(new { message = $"Image not found: {decodedImageName}" });
            }

            // Parse the base64 string to get content type and data
            var base64Data = table.ImageBase64;

            // Check if it includes the data URI prefix (e.g., "data:image/jpeg;base64,")
            if (base64Data.StartsWith("data:"))
            {
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex > 0)
                {
                    var metaData = base64Data.Substring(0, commaIndex);
                    base64Data = base64Data.Substring(commaIndex + 1);

                    // Extract content type from metadata
                    var contentType = "image/jpeg"; // Default
                    if (metaData.Contains("image/png"))
                        contentType = "image/png";
                    else if (metaData.Contains("image/gif"))
                        contentType = "image/gif";
                    else if (metaData.Contains("image/webp"))
                        contentType = "image/webp";
                    else if (metaData.Contains("image/bmp"))
                        contentType = "image/bmp";

                    var imageBytes = Convert.FromBase64String(base64Data);
                    return File(imageBytes, contentType);
                }
            }

            // If no data URI prefix, assume it's plain base64
            var bytes = Convert.FromBase64String(base64Data);
            return File(bytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error retrieving image: {ex.Message}" });
        }
    }

    [HttpGet("food/{*imageName}")]
    public async Task<IActionResult> GetFoodImage(string imageName)
    {
        try
        {
            // URL decode the image name and trim any leading slashes
            var decodedImageName = Uri.UnescapeDataString(imageName).TrimStart('/');

            // The database stores just the filename
            var food = await _context.Foods
                .FirstOrDefaultAsync(f => f.Image == decodedImageName);

            if (food == null || string.IsNullOrEmpty(food.ImageBase64))
            {
                return NotFound(new { message = $"Image not found: {decodedImageName}" });
            }

            // Parse the base64 string to get content type and data
            var base64Data = food.ImageBase64;

            // Check if it includes the data URI prefix (e.g., "data:image/jpeg;base64,")
            if (base64Data.StartsWith("data:"))
            {
                var commaIndex = base64Data.IndexOf(',');
                if (commaIndex > 0)
                {
                    var metaData = base64Data.Substring(0, commaIndex);
                    base64Data = base64Data.Substring(commaIndex + 1);

                    // Extract content type from metadata
                    var contentType = "image/jpeg"; // Default
                    if (metaData.Contains("image/png"))
                        contentType = "image/png";
                    else if (metaData.Contains("image/gif"))
                        contentType = "image/gif";
                    else if (metaData.Contains("image/webp"))
                        contentType = "image/webp";
                    else if (metaData.Contains("image/bmp"))
                        contentType = "image/bmp";

                    var imageBytes = Convert.FromBase64String(base64Data);
                    return File(imageBytes, contentType);
                }
            }

            // If no data URI prefix, assume it's plain base64
            var bytes = Convert.FromBase64String(base64Data);
            return File(bytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error retrieving image: {ex.Message}" });
        }
    }
}
