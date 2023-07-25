using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
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
    
    [HttpGet(Name = "GetAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<ActionResult<IEnumerable<UserDTO>>>(users);
        
    }
    
    [HttpGet("{id:int}", Name = "GetUser")]
    public async Task<ActionResult<UserDTO>> Get(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        return _mapper.Map<UserDTO>(user);

    }
}