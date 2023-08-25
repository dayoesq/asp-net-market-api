using AutoMapper;
using Market.DataContext;
using Market.Filters;
using Market.Models;
using Market.Models.DTOS;
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

    [HttpGet(Name = "all-products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
    
    [HttpPost(Name = "create-product")]
    [ValidateImageAndVideoFilter]
    public async Task<IActionResult>  CreateProduct([FromForm] ProductCreateDto productCreateDto)
    {
        var product = _mapper.Map<Product>(productCreateDto);

        foreach (var (imageDto, index) in productCreateDto.Images.Select((value, index) => (value, index)))
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

        return Created(nameof(CreateProduct), product);

    }
    
    private static string GenerateUniqueFileName(int productId, int index, string? fileExtension)
    {
        var guid = Guid.NewGuid().ToString();
        var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{guid}-{date}-P{productId}-I{index + 1}.{fileExtension}";
    }

    

}