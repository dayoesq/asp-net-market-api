using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Models.DTOS.Discounts;
using Market.Repositories;
using Market.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Controllers;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
[ApiController]
[Route("[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRepository<Discount, int> _discountRepository;
    private readonly IUnitOfWork _unitOfWork;


    public DiscountsController(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Discount, int> discountRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _discountRepository = discountRepository;
    }

    //[HttpPost(Name = "create-discount")]
    public async Task<IActionResult> CreateDiscount([FromBody] DiscountUpsertDto model)
    {
        var existingDiscount = await _discountRepository.GetAsync(d => d.Code.ToUpper() == model.Code.ToUpper());
        if (existingDiscount != null) return Conflict(new ErrorResponse(Errors.Conflict409));
        var discount = _mapper.Map<Discount>(model);
        var newDiscount = _discountRepository.Create(discount);
        await _unitOfWork.CommitAsync();
        return CreatedAtAction(nameof(CreateDiscount), new { id = newDiscount.Id }, discount);
    }

    [AllowAnonymous]
    [HttpGet(Name = "get-discounts")]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await _discountRepository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<Discount>, IEnumerable<DiscountDto>>(discounts);
        return Ok(result);

    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "get-discount")]
    public async Task<IActionResult> GetDiscount(int id)
    {
        var discount = await _discountRepository.GetAsync(d => d.Id == id);
        if (discount == null) return NotFound(new { message = Errors.NotFound404 });
        var result = _mapper.Map<Discount, DiscountDto>(discount);
        return Ok(result);
    }

    [HttpPut("{id:int}", Name = "update-discount")]
    public async Task<IActionResult> UpdateDiscount(int id, [FromBody] DiscountUpsertDto model)
    {
        var existingDiscount = await _discountRepository.GetAsync(d => d.Id == id);
        if (existingDiscount == null) return NotFound(new ErrorResponse(Errors.Conflict409));
        if (existingDiscount.Code.ToUpper() == model.Code.ToUpper()) return Conflict(new { message = Errors.Conflict409 });
        var result = _mapper.Map(model, existingDiscount);
        _discountRepository.Update(id, result);
        await _unitOfWork.CommitAsync();
        return Ok(result);

    }

    [HttpDelete("{id:int}", Name = "delete-discount")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var discountIsDeleted = await _discountRepository.DeleteAsync(d => d.Id == id);
        if (!discountIsDeleted) return NotFound(new { message = Errors.NotFound404 });
        await _unitOfWork.CommitAsync();
        return Ok(new { message = ResponseMessage.Success });
    }

}
