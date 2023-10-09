using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Models.DTOS.Brands;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;
using Market.Filters;

namespace Market.Controllers;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<Brand, string> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;


    public BrandsController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Brand, string> brandRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _brandRepository = brandRepository;
    }

    [HttpPost(Name = "create-brand")]
    [TrimRequestStrings]
    public async Task<IActionResult> CreateBrand([FromBody] BrandUpsertDto model)
    {
        var existingBrand = await _brandRepository.GetAsync(b => b.Name.ToUpper() == model.Name.ToUpper());
        if (existingBrand != null) return Conflict(new ErrorResponse(Errors.Conflict409));
        var brand = _mapper.Map<Brand>(model);
        var newBrand = _brandRepository.Create(brand);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateBrand), new { id = newBrand.Id }, brand);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-brands")]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _brandRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Brand>, IEnumerable<BrandDto>>(brands);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-brand")]
    public async Task<IActionResult> GetBrand(int id)
    {
        var brand = await _brandRepository.GetAsync(b => b.Id == id);
        if (brand == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<Brand, BrandDto>(brand);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-brand")]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandUpsertDto model)
    {
        var existingBrand = await _brandRepository.GetAsync(b => b.Id == id);
        if (existingBrand == null) return NotFound(new { message = Errors.NotFound404 });
        if (existingBrand.Name == model.Name.ToUpper()) return Conflict(new { message = Errors.Conflict409 });
        var result = _mapper.Map(model, existingBrand);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    [HttpDelete("{id:int}", Name = "delete-brand")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var existingBrand = await _brandRepository.GetAsync(b => b.Id == id);
        if (existingBrand == null) return NotFound(new { message = Errors.NotFound404 });
        await _brandRepository.DeleteAsync(b => b.Id == id);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}
