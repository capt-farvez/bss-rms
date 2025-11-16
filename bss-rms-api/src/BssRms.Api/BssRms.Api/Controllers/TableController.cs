using BssRms.Application.DTOs.Table;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTableDto dto)
    {
        try
        {
            var result = await _tableService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is InvalidOperationException)
                return Conflict(new { message = ex.Message });

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
            var result = await _tableService.GetDatatableAsync(Page, Per_Page, Search, Sort);
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
            var result = await _tableService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _tableService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Table with ID {id} not found." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTableDto dto)
    {
        try
        {
            var result = await _tableService.UpdateAsync(id, dto);
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
            var result = await _tableService.DeleteAsync(id);
            return Ok(new { message = "Table deleted successfully.", deleted = result });
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
