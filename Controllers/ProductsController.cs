using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;
using Market.Models.DTOS.Products;
using Market.Filters;

namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHost;
    private readonly IMapper _mapper;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Product, int> productRepository, IWebHostEnvironment webHost)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _webHost = webHost;
    }

    

    [AllowAnonymous]
    [HttpGet(Name = "get-Products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(products);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-Product")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productRepository.GetAsync(id);
        if (product == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<Product, ProductDto>(product);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-Product")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpsertDto model)
    {
        var existingProduct = await _productRepository.GetAsync(id);
        if (existingProduct == null) return NotFound(new { message = Errors.NotFound404 });
        if (existingProduct.Name == model.Name.ToUpper()) return Conflict(new { message = Errors.Conflict409 });
        var result = _mapper.Map(model, existingProduct);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    [HttpDelete("{id:int}", Name = "delete-Product")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var existingProduct = await _productRepository.GetAsync(id);
        if (existingProduct == null) return NotFound(new { message = Errors.NotFound404 });
        await _productRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}