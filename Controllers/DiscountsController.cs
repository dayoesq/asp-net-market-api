using AutoMapper;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Market.DataContext;
using Market.Models;

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

        
        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountCreateDto discountCreateDto)
        {
            var currentDiscount =
                await _context.Discounts.FirstOrDefaultAsync(a => a.Code == discountCreateDto.Code.ToUpper());

            if (currentDiscount != null)
            {
                return Conflict();
            }

            var discount = _mapper.Map<Discount>(discountCreateDto);
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateDiscount), new { id = discount.Id }, discount);
        }


        
        [HttpGet]
        public async Task<IActionResult> GetDiscounts()
        {
            var discounts = await _context.Discounts.ToListAsync();
            var discountDtos = _mapper.Map<IEnumerable<Discount>, IEnumerable<DiscountDto>>(discounts);
            return Ok(discountDtos);

        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDiscount(int id)
        {
            var currentDiscount = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);
            if (currentDiscount == null)
            {
                return NotFound();
            }

            var discount = _mapper.Map<Discount, DiscountDto>(currentDiscount);
            return Ok(discount);
        }
        

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] DiscountUpdateDto discountUpdateDto)
        {
            var model = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            if (discountUpdateDto.Code == model.Code.ToUpper())
            {
                return Conflict();
            }
            
            var result = _mapper.Map(discountUpdateDto, model);
            await _context.SaveChangesAsync();
            return Ok(result);
            
        }

        
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var model = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);
            if (model == null) return NotFound();
            _context.Discounts.Remove(model);
            await _context.SaveChangesAsync();
            return Ok(model.Id);
        }
        
    }
