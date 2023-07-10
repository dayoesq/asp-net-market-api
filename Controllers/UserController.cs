using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.Dtos;
using Market.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.Controllers;
[Route(Constants.apiUserPrefix)]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> _logger;
    private readonly Mapper _mapper;

    public UserController(ApplicationDbContext context, ILogger<UserController> logger, Mapper mapper)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
    }
    
    [HttpPost(Name = "RegisterUser")]
    public async Task<ActionResult<UserCreationDto>> Register([FromBody] UserCreationDto userCreationDto)
    {
        var user = _mapper.Map<User>(userCreationDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"User with {user.Id} has been created");
        return NoContent();

    }

    [HttpGet(Name = "GetAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> Get()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<ActionResult<IEnumerable<UserDto>>>(users);
        
    }
    
    [HttpGet("{id:int}", Name = "GetUser")]
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        return _mapper.Map<UserDto>(user);

    }
}