using AutoMapper;
using Market.Models;
using Market.Models.DTOS.Sizes;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class SizesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Size, int> _sizeRepository;
    private readonly IMapper _mapper;

    public SizesController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Size, int> sizeRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _sizeRepository = sizeRepository;
    }

    [HttpPost(Name = "create-size")]
    public async Task<IActionResult> CreateSize([FromBody] SizeUpsertDto model)
    {
        var existingSize = await _sizeRepository.GetAsync(s => s.ProductSize == model.ProductSize);
        if(existingSize != null) return Conflict(new ErrorResponse(Errors.Conflict409));
        var size = _mapper.Map<Size>(model);
        var newSize = await _sizeRepository.CreateAsync(size);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateSize), new { id = newSize.Id }, size);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-sizes")]
    public async Task<IActionResult> GetSizes()
    {
        var sizes = await _sizeRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<SizeDto>>(sizes);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = "get-size")]
    public async Task<IActionResult> GetSize(int id)
    {
        var size = await _sizeRepository.GetAsync(s => s.Id == id);
        if (size == null) return NotFound(new ErrorResponse(Errors.NotFound404));
        var result = _mapper.Map<SizeDto>(size);
        return Ok(result);

    }

    [HttpDelete("{id}", Name = "delete-size")]
    public async Task<IActionResult> DeleteSize(int id)
    {
        var size = await _sizeRepository.DeleteAsync(s => s.Id == id);
        if (!size) return NotFound(new ErrorResponse(Errors.NotFound404));
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });

    }
}