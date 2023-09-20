using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Market.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProductsController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet(Name = "products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        var result = _mapper.Map<List<ProductDto>>(products);
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = "product")]
    public async Task<IActionResult> GetProduct(long id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { message = Errors.NotFound404 });
        }
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpPost(Name = "create-product")]
    public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto model)
    {
        var product = _mapper.Map<Product>(model);

        foreach (var (imageDto, index) in model.Images.Select((value, index) => (value, index)))
        {
            var fileExtension = Path.GetExtension(imageDto.Path)?.TrimStart('.');
            var uniqueFileName = GenerateUniqueFileName(product.Id, index + 1, fileExtension);
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/products", uniqueFileName);

            using var client = new HttpClient();
            using var response = await client.GetAsync(imageDto.Path);
            await using var stream = await response.Content.ReadAsStreamAsync();

            await using var fileStream = new FileStream(imagePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            var productImage = new ProductImage
            {
                Name = imageDto.Name,
                Path = $"/images/products/{uniqueFileName}"
            };

            product.Images.Add(productImage);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateProduct), product);

    }

    private static string GenerateUniqueFileName(int productId, int index, string? fileExtension)
    {
        var guid = Guid.NewGuid().ToString();
        var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{guid}-{date}-P{productId}-I{index + 1}.{fileExtension}";
    }



}