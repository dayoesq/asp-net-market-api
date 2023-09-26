using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Models.DTOS.Categories;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;


namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<Category, int> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;


    public CategoriesController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Category, int> categoryRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    [HttpPost(Name = "create-category")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryUpsertDto model)
    {
        var category = _mapper.Map<Category>(model);
        var createdCategory = await _categoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateCategory), new { id = createdCategory.Id }, category);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(categories);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-category")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryRepository.GetAsync(id);
        if (category == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<Category, CategoryDto>(category);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-category")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpsertDto model)
    {
        var existingCategory = await _categoryRepository.GetAsync(id);
        if (existingCategory == null) return NotFound(new { message = Errors.NotFound404 });
        if (existingCategory.Name == model.Name.ToUpper()) return Conflict(new { message = Errors.Conflict409 });
        var result = _mapper.Map(model, existingCategory);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    [HttpDelete("{id:int}", Name = "delete-category")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var existingCategory = await _categoryRepository.GetAsync(id);
        if (existingCategory == null) return NotFound(new { message = Errors.NotFound404 });
        await _categoryRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}
