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
using Market.Models.DTOS.Errors;

namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHost;
    private readonly IMapper _mapper;
    private readonly IRepository<Product, int> _productRepository;
    private readonly IRepository<Image, int> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public ProductsController(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRepository<Product, int> productRepository,
        IRepository<Image, int> imageRepository,
         IWebHostEnvironment webHost,
         ILogger logger
         )
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _imageRepository = imageRepository;
        _webHost = webHost;
        _logger = logger;
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
        try
        {
            var product = await _productRepository.GetAsync(id);
            var result = _mapper.Map<Product, ProductDto>(product!);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{id:int}", Name = "update-Product")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpsertDto model, List<IFormFile> files)
    {
        try
        {
            var existingProduct = await _productRepository.GetAsync(id);
            var result = _mapper.Map(model, existingProduct);

            await _unitOfWork.CommitAsync();

            var productImageFolder = Path.Combine(_webHost.WebRootPath, "ProductImages", $"{existingProduct!.Name}-{existingProduct.Id}");
            if (!Directory.Exists(productImageFolder)) Directory.CreateDirectory(productImageFolder);

            var imageUrls = new List<string>();

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
                    await _imageRepository.CreateAsync(image);

                    imageUrls.Add(Path.Combine("/productImages", $"{existingProduct.Name}-{existingProduct.Id}", fileName));
                    existingProduct.ImageUrls.Add(image.Url);
                }
            }

            await _unitOfWork.CommitAsync();

            model.ImageUrls = imageUrls;

            return Ok(model);
        }
        catch (Exception e)
        {
            _logger.LogError($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [HttpDelete("{id:int}", Name = "delete-Product")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _productRepository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
            return Ok(new { message = ResponseMessage.Success });
        }
        catch (Exception e)
        {
            _logger.LogError($"{Errors.Server500}-{e.Message}");
            return (IActionResult)new ErrorResponse { Message = Errors.Server500 };
        }
    }

}