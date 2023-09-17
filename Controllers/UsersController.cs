using AutoMapper;
using Market.DataContext;
using Market.Models.DTOS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    
    public UsersController(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }
    
   
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var identity = _contextAccessor.HttpContext?.User.Identity;
        if (identity is { IsAuthenticated: true })
        {
            // User is authenticated, you can access claims, roles, and other identity properties.
            var userName = identity.Name;
            var isAuthenticated = identity.IsAuthenticated;
            // ...
        }
        else
        {
            Console.WriteLine("No authenticated!!!");
        }

        var users = await _context.Users.ToListAsync();
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return Ok(userDtos);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        var userDto = _mapper.Map<UserDto>(user);
            
        return Ok(userDto);

    }
}