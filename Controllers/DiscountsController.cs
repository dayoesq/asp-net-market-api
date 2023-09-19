using AutoMapper;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Market.DataContext;
using Market.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;


    public DiscountsController(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountCreateDto model)
    {
        var discount =
            await _context.Discounts.FirstOrDefaultAsync(d => d.Code == model.Code!.ToUpper());

        if (discount != null)
        {
            return Conflict();
        }

        var result = _mapper.Map<Discount>(model);
        _context.Discounts.Add(result);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateDiscount), new { id = result.Id }, discount);
    }

    [HttpGet]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await _context.Discounts.ToListAsync();
        var result = _mapper.Map<IEnumerable<Discount>, IEnumerable<DiscountDto>>(discounts);
        return Ok(result);

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDiscount(int id)
    {
        var discount = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);
        if (discount == null)
        {
            return NotFound();
        }

        var result = _mapper.Map<Discount, DiscountDto>(discount);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDiscount(int id, [FromBody] DiscountUpdateDto model)
    {
        var category = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        if (category.Code == model.Code.ToUpper())
        {
            return Conflict();
        }

        var result = _mapper.Map(model, category);
        await _context.SaveChangesAsync();
        return Ok(result);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var category = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);
        if (category == null) return NotFound();
        _context.Discounts.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(category.Id);
    }

}
