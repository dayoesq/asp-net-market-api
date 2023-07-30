using AutoMapper;
using Market.DataContext;
using Market.Models.DTOS;
using Market.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public async Task<ActionResult<IEnumerable<UserDto>>> Get()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<ActionResult<IEnumerable<UserDto>>>(users);
        
    }
    
    [Authorize]
    [HttpGet("{Id}", Name = "GetUser")]
    public async Task<ActionResult<UserDto>> Get(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        return _mapper.Map<UserDto>(user);

    }
}