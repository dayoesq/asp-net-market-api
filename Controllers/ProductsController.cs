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

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHost;
    private readonly IMapper _mapper;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IRepository<Image, int> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRepository<Product, int> productRepository,
        IRepository<Image, int> imageRepository,
         IWebHostEnvironment webHost
         )
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _imageRepository = imageRepository;
        _webHost = webHost;
    }

    [HttpPost(Name = "create-product")]
    [TrimRequestStrings]
    public async Task<IActionResult> CreateProduct([FromForm] ProductUpsertDto model, List<IFormFile>? files)
    {
        var existingProduct = await _productRepository.GetAsync(p => p.Name.ToUpper() == model.Name.ToUpper());
        if (existingProduct != null) return Conflict(new ErrorResponse(Errors.Conflict409));

        var result = _mapper.Map(model, existingProduct);

        await _unitOfWork.CommitAsync();

        var productImageFolder = Path.Combine(_webHost.WebRootPath, "images", "products", $"{existingProduct!.Name}-{existingProduct.Id}");
        if (!Directory.Exists(productImageFolder)) Directory.CreateDirectory(productImageFolder);

        var imageUrls = new List<string>();

        if (files != null)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(productImageFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var image = new Image { Url = fileName, ProductId = result!.Id };
                    _imageRepository.Create(image);

                    imageUrls.Add(Path.Combine("/images/products", $"{existingProduct.Name}-{existingProduct.Id}", fileName));
                    existingProduct.ImageUrls.Add(image.Url);
                }
            }
        }

        await _unitOfWork.CommitAsync();

        model.ImageUrls = imageUrls;

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(products);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-product")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productRepository.GetAsync(p => p.Id == id);
        var result = _mapper.Map<Product, ProductDto>(product!);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-product")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpsertDto model, List<IFormFile>? files)
    {
        var existingProduct = await _productRepository.GetAsync(p => p.Id == id);
        var result = _mapper.Map(model, existingProduct);

        await _unitOfWork.CommitAsync();

        var productImageFolder = Path.Combine(_webHost.WebRootPath, "images", "products", $"{existingProduct!.Name}-{existingProduct.Id}");
        if (!Directory.Exists(productImageFolder)) Directory.CreateDirectory(productImageFolder);

        var imageUrls = new List<string>();

        if (files != null)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(productImageFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var image = new Image { Url = fileName, ProductId = result!.Id };
                    _imageRepository.Create(image);

                    imageUrls.Add(Path.Combine("/images/products", $"{existingProduct.Name}-{existingProduct.Id}", fileName));
                    existingProduct.ImageUrls.Add(image.Url);
                }
            }
        }

        await _unitOfWork.CommitAsync();

        model.ImageUrls = imageUrls;

        return Ok(model);
    }


    [HttpDelete("{id:int}", Name = "delete-product")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productRepository.DeleteAsync(p => p.Id == id);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}