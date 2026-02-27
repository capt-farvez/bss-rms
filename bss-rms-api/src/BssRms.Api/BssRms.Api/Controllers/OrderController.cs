using System.Security.Claims;
using BssRms.Application.DTOs.Order;
using BssRms.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BssRms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var result = await _orderService.CreateAsync(dto);
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
            var result = await _orderService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Order with ID {id} not found." });

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
            var result = await _orderService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <param name="Page">Page number (default: 1)</param>
    /// <param name="Per_Page">Items per page (default: 10)</param>
    /// <param name="Search">Search by order number, table number, or phone number</param>
    /// <param name="Sort">Sort field: ordernumber, amount, orderdate, createdat, status (prefix with - for descending, e.g. -status)</param>
    /// <param name="Status">Filter by order status: 0 = Pending, 1 = Confirmed, 2 = Preparing, 3 = PreparedToServe, 4 = Served, 5 = Paid</param>
    [HttpGet("datatable")]
    public async Task<IActionResult> GetDatatable(
        [FromQuery] int Page = 1,
        [FromQuery] int Per_Page = 10,
        [FromQuery] string? Search = null,
        [FromQuery] string? Sort = null,
        [FromQuery] int? Status = null)
    {
        try
        {
            var result = await _orderService.GetDatatableAsync(Page, Per_Page, Search, Sort, Status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto dto)
    {
        try
        {
            var result = await _orderService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update-status/{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var result = await _orderService.UpdateStatusAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _orderService.DeleteAsync(id);
            return Ok(new { message = "Order deleted successfully." });
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
