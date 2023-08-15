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
    public ProductsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(Name = "all-products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
    
    // [HttpPost(Name = "create-product")]
    // [ValidateImageAndVideoFilter]
    // public async Task<IActionResult>  CreateProduct([FromForm] ProductDto productDto)
    // {
    //     
    // }
    
    


}