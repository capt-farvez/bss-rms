using BssRms.Application.DTOs;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestTableController : ControllerBase
{
    private readonly ITestTableService _testTableService;

    public TestTableController(ITestTableService testTableService)
    {
        _testTableService = testTableService;
    }

    // Get all test table records
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestTableDto>>> GetAll()
    {
        var result = await _testTableService.GetAllAsync();
        return Ok(result);
    }

    // Get a specific test table record by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<TestTableDto>> GetById(int id)
    {
        var result = await _testTableService.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound(new { message = $"TestTable with ID {id} not found" });
        }

        return Ok(result);
    }

    // Create a new test table record
    [HttpPost]
    public async Task<ActionResult<TestTableDto>> Create([FromBody] CreateTestTableDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _testTableService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
