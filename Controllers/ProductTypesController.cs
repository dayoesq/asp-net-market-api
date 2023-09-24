using AutoMapper;
using Market.Models;
using Market.Models.DTOS.ProductTypes;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class ProductTypesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ProductType, int> _productTypeRepository;
    private readonly IMapper _mapper;

    public ProductTypesController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<ProductType, int> productTypeRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _productTypeRepository = productTypeRepository;
    }

    [HttpPost(Name = "create-product-type")]
    public async Task<IActionResult> CreateProductType([FromBody] ProductTypeUpsertDto model)
    {
        var productType = _mapper.Map<ProductType>(model);
        var createdProductType = await _productTypeRepository.CreateAsync(productType);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateProductType), new { id = createdProductType.Id }, createdProductType);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-product-types")]
    public async Task<IActionResult> GetProductTypes()
    {
        var productTypes = await _productTypeRepository.GetAllAsync();
        var result = _mapper.Map<List<ProductTypeDto>>(productTypes);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = "get-product-type")]
    public async Task<IActionResult> GetProductType(int id)
    {
        var productType = await _productTypeRepository.GetAsync(id);
        if (productType == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<ProductTypeDto>(productType);
        return Ok(result);

    }

    [HttpPut("{id:int}", Name = "update-product-type")]
    public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeUpsertDto model)
    {
        var existingProductType = await _productTypeRepository.GetAsync(id);
        if (existingProductType == null) return NotFound(new { message = Errors.NotFound404 });
        var productType = _mapper.Map<ProductType>(model);
        var createdProductType = await _productTypeRepository.CreateAsync(productType);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateProductType), new { id = createdProductType.Id }, createdProductType);
    }


    [HttpDelete("{id}", Name = "delete-product-type")]
    public async Task<IActionResult> DeleteProductType(int id)
    {
        var productType = await _productTypeRepository.DeleteAsync(id);
        if (!productType) return NotFound(new { message = Errors.NotFound404 });
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });

    }
}