using AutoMapper;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Market.DataContext;
using Market.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public CategoriesController(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
    [HttpPost(Name = "create-category")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto model)
    {
        var category =
            await _context.Categories.FirstOrDefaultAsync(a => a.Name == model.Name.ToUpper());

        if (category != null)
        {
            return Conflict(new { message = Errors.Conflict409 });
        }

        var result = _mapper.Map<Category>(category);
        _context.Categories.Add(result);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateCategory), new { id = result.Id }, category);
    }

    [HttpGet(Name = "get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        var result = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(categories);
        return Ok(result);

    }

    [HttpGet("{id:int}", Name = "get-category")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);
        if (category == null)
        {
            return NotFound(new { message = Errors.NotFound404 });
        }

        var result = _mapper.Map<Category, CategoryDto>(category);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
    [HttpPut("{id:int}", Name = "update-category")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto model)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);

        if (category == null)
        {
            return NotFound(new { message = Errors.NotFound404 });
        }

        if (model.Name.ToUpper() == category.Name.ToUpper())
        {
            return Conflict(new { message = Errors.Conflict409 });
        }

        var result = _mapper.Map(model, category);
        await _context.SaveChangesAsync();
        return Ok(result);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.Super)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var category = await _context.Discounts.FirstOrDefaultAsync(a => a.Id == id);
        if (category == null) return NotFound(new { message = Errors.NotFound404 });
        _context.Discounts.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(category.Id);
    }

}
