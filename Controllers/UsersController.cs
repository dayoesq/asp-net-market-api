using AutoMapper;
using Market.DataContext;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers;
[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly Mapper _mapper;
    
    public UsersController(ApplicationDbContext context, Mapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Constants.ADMIN)]
    [Authorize(Roles = "admin, super")]
    [HttpGet(Name = "GetAllUsers")]
    public async Task<IActionResult> Get()
    {
        var users = await _context.Users.ToListAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return Ok(userDtos);
    }
    
    [Authorize]
    [HttpGet("{Id}", Name = "GetUser")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? NotFound() : _mapper.Map<IActionResult>(user);
    }
}