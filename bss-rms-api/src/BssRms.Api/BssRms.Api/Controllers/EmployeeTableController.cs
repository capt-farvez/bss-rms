using BssRms.Application.DTOs.EmployeeTable;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeTableController : ControllerBase
{
    private readonly IEmployeeTableService _employeeTableService;

    public EmployeeTableController(IEmployeeTableService employeeTableService)
    {
        _employeeTableService = employeeTableService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeTableDto dto)
    {
        try
        {
            var result = await _employeeTableService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is InvalidOperationException)
                return Conflict(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-range")]
    public async Task<IActionResult> CreateRange([FromBody] List<CreateEmployeeTableDto> dtos)
    {
        try
        {
            var result = await _employeeTableService.CreateRangeAsync(dtos);
            return Ok(new { message = result });
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
            var result = await _employeeTableService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"EmployeeTable with ID {id} not found." });

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
            var result = await _employeeTableService.GetAllAsync();
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
            var result = await _employeeTableService.GetDatatableAsync(Page, Per_Page, Search, Sort);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeTableDto dto)
    {
        try
        {
            var result = await _employeeTableService.UpdateAsync(id, dto);
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
            var result = await _employeeTableService.DeleteAsync(id);
            return Ok(new { message = "Employee table assignment deleted successfully." });
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
