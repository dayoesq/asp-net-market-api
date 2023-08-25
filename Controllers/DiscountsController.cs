using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public DiscountsController(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> CreateDiscount(DiscountCreateDto discountCreateDto)
    {
        var currentDiscount = _context.Discounts.Where(a => a.Code == discountCreateDto.Code.ToUpper());
        var discount = _mapper.Map<Discount>(discountCreateDto);
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();

        var createdDto = _mapper.Map<DiscountDto>(discount);

        return CreatedAtAction(nameof(GetDiscount), new { id = createdDto.Id }, createdDto);
    }
    
    
    [HttpGet(Name = "get-discounts")]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await _context.Discounts.ToListAsync();
        var model = _mapper.Map<List<DiscountDto>>(discounts);
        return Ok(model);
    }
    
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountDto>> GetDiscount(int id)
    {
        var discount = await _context.Discounts.Where(a => a.Id == id).FirstOrDefaultAsync();
        
        if (discount == null)
        {
            return NotFound();
        }

        return _mapper.Map<DiscountDto>(discount);
    }
    
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscount(int id, DiscountDto discountDto)
    {
        var currentDiscount = await _context.Discounts.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (currentDiscount == null)
        {
            return NotFound();
        }

        var discount = _mapper.Map<Discount>(discountDto);
        _context.Entry(discount).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DiscountExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return Ok(discount);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var discount = await _context.Discounts.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (discount == null)
        {
            return NotFound();
        }

        _context.Discounts.Remove(discount);
        await _context.SaveChangesAsync();

        return Ok(); 
    }
    
    private bool DiscountExists(int id)
    {
        return _context.Discounts.Any(e => e.Id == id);
    }

    
   
}