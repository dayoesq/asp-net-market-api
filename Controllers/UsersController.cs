using AutoMapper;
using Market.Models;
using Market.Models.DTOS.Users;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Market.Utils;
using Market.Utils.Constants;
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
    private readonly IWebHostEnvironment _webHost;

    public UsersController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<ApplicationUser, string> userRepository, IWebHostEnvironment webHost)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _webHost = webHost;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.Admin)]
    [HttpGet(Name = "get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<UserDto>>(users);
        return Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}", Name = "get-user")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userRepository.GetAsync(u => u.Id == id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<UserDto>(user);
        return Ok(result);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{id}", Name = "update-user")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto model)
    {
        var user = await _userRepository.GetAsync(u => u.Id == id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
        var mappedUser = _mapper.Map(model, user);
        _userRepository.Update(id, mappedUser);
        var result = _mapper.Map<UserDto>(mappedUser);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPatch("{id}", Name = "update-user-avatar")]
    public async Task<IActionResult> UpdateUserAvatar(string id, [FromForm] UserAvatarUpdateDto avatar)
    {
        var user = await _userRepository.GetAsync(u => u.Id == id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
        
        var userImageFolder = Path.Combine(_webHost.WebRootPath, "userImages");
        if (!Directory.Exists(userImageFolder)) Directory.CreateDirectory(userImageFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.File.FileName);
        var filePath = Path.Combine(userImageFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.File.CopyToAsync(stream);
        }
        user.AvatarUrl = Path.Combine("/userImages", fileName);
        await _unitOfWork.CommitAsync();

        var result = _mapper.Map<UserDto>(user);
        return Ok(result);

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}", Name = "delete-user")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userRepository.DeleteAsync(u => u.Id == id);
        if (!user) return NotFound(new { message = Errors.NotFound404 });
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });

    }
}