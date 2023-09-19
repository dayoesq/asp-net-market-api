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
public class CategoriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;


    public CategoriesController(IMapper mapper, ApplicationDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPost(Name = "create-category")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto model)
    {
        var existingCategory =
            await _context.Categories.FirstOrDefaultAsync(a => a.Name == model.Name.ToUpper());

        if (existingCategory != null)
        {
            return Conflict();
        }

        var category = _mapper.Map<Category>(existingCategory);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(CreateCategory), new { id = category.Id }, category);
    }

    [HttpGet(Name = "get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        var categoryDtos = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(categories);
        return Ok(categoryDtos);

    }

    [HttpGet("{id:int}", Name = "get-category")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);
        if (existingCategory == null)
        {
            return NotFound();
        }

        var categoryDto = _mapper.Map<Category, CategoryDto>(existingCategory);
        return Ok(categoryDto);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPut("{id:int}", Name = "update-category")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto model)
    {
        var existingCategory = await _context.Categories.FirstOrDefaultAsync(a => a.Id == id);

        if (existingCategory == null)
        {
            return NotFound();
        }

        if (model.Name.ToUpper() == existingCategory.Name.ToUpper())
        {
            return Conflict();
        }

        var categoryDto = _mapper.Map(model, existingCategory);
        await _context.SaveChangesAsync();
        return Ok(categoryDto);

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
