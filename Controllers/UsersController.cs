using AutoMapper;
using Market.Models;
using Market.Models.DTOS;
using Market.Models.DTOS.Users;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Market.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ApplicationUser, string> _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<ApplicationUser, string> userRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [HttpGet(Name = "get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var result = _mapper.Map<List<UserDto>>(users);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}", Name = "get-user")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userRepository.GetAsync(id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<UserDto>(user);
        return Ok(result);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}", Name = "delete-user")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userRepository.DeleteAsync(id);
        if (!user) return NotFound(new { message = Errors.NotFound404 });
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });

    }
}