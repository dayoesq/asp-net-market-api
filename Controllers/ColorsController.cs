using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Models.DTOS.Colors;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class ColorsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<Color, int> _colorRepository;
    private readonly IUnitOfWork _unitOfWork;


    public ColorsController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Color, int> colorRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _colorRepository = colorRepository;
    }

    [HttpPost(Name = "create-color")]
    public async Task<IActionResult> CreateColor([FromBody] ColorUpsertDto model)
    {
        var color = _mapper.Map<Color>(model);
        var createdColor = _colorRepository.Create(color);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateColor), new { id = createdColor.Id }, color);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-colors")]
    public async Task<IActionResult> GetColors()
    {
        var colors = await _colorRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Color>, IEnumerable<ColorDto>>(colors);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-color")]
    public async Task<IActionResult> GetColor(int id)
    {
        var color = await _colorRepository.GetAsync(c => c.Id == id);
        if (color == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<Color, ColorDto>(color);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-color")]
    public async Task<IActionResult> UpdateColor(int id, [FromBody] ColorUpsertDto model)
    {
        var existingColor = await _colorRepository.GetAsync(c => c.Id == id);
        if (existingColor == null) return NotFound(new { message = Errors.NotFound404 });
        if (existingColor.Name == model.Name.ToUpper()) return Conflict(new { message = Errors.Conflict409 });
        var result = _mapper.Map(model, existingColor);
        _colorRepository.Update(id, result);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    [HttpDelete("{id:int}", Name = "delete-color")]
    public async Task<IActionResult> DeleteColor(int id)
    {
        var existingColor = await _colorRepository.GetAsync(c => c.Id == id);
        if (existingColor == null) return NotFound(new { message = Errors.NotFound404 });
        await _colorRepository.DeleteAsync(c => c.Id == id);
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}
