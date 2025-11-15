using BssRms.Application.DTOs.Food;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateFoodDto dto)
    {
        try
        {
            var result = await _foodService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is InvalidOperationException)
                return Conflict(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _foodService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Food with ID {id} not found." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _foodService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("datatable")]
    public async Task<IActionResult> GetDatatable(
        [FromQuery] int Page = 1,
        [FromQuery] int Per_Page = 10,
        [FromQuery] string? Search = null,
        [FromQuery] string? Sort = null)
    {
        try
        {
            var result = await _foodService.GetDatatableAsync(Page, Per_Page, Search, Sort);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFoodDto dto)
    {
        try
        {
            var result = await _foodService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            if (ex is InvalidOperationException)
                return Conflict(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _foodService.DeleteAsync(id);
            return Ok(new { message = "Food deleted successfully." });
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
