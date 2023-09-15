using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Market.OptionsSetup.Jwt;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ClaimTypes = Market.Utils.ClaimTypes;

namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private const string LoginMode = "loginMode";
    private const string RegisterMode = "registerMode";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly JwtOptions _jwtOptions;


    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper, ApplicationDbContext context, JwtOptions jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _context = context;
        _jwtOptions = jwtOptions;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

        if (!Regex.IsMatch(registerDto.FirstName, Constants.NamePattern) &&
            !Regex.IsMatch(registerDto.LastName, Constants.NamePattern))
        {
            return BadRequest(ErrorResponse.SendMessage(Errors.InvalidFormat + " name format"));
        }

        var currentUser = _userManager.FindByEmailAsync(registerDto.Email);
        if (currentUser.Result?.Email?.ToUpperInvariant() == registerDto.Email.ToUpperInvariant())
        {
            return Conflict(ErrorResponse.SendMessage(Errors.Conflict409));
        }

        var user = _mapper.Map<ApplicationUser>(registerDto);
        user.VerificationCode = Helper.GenerateRandomNumber(8);
        user.IsVerified = false;
        user.EmailConfirmed = false;
        
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, Roles.User),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Uuid, new Guid().ToString())
        };
        
        await _userManager.AddClaimsAsync(user, claims);
        await _context.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto auth)
    {
        var user = await _userManager.FindByEmailAsync(auth.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
        {
            return Unauthorized(ErrorResponse.SendMessage(Errors.InvalidCredentials));
        }

        if (!user.EmailConfirmed)
        {
            return BadRequest(ErrorResponse.SendMessage(Errors.UnverifiedAccount));
        }

        var result =
            await _signInManager.PasswordSignInAsync(user, auth.Password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ErrorResponse.SendMessage(Errors.Server500));
        }
        
        var claims = await _userManager.GetClaimsAsync(user);
        
        var token = GenerateJwtToken(user, claims);
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(new { Token = token });


    }


    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    //[Authorize(Roles = "ADMIN, SUPER")]
    [HttpPost("assign-claim/{id}")]
    public async Task<IActionResult> AssignClaim(string id, [FromBody] ClaimDto claimDto)
    {

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound(ErrorResponse.SendMessage(Errors.NotFound404));
        }

        await _userManager.AddClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
        return Ok();
    }


    //[Authorize(Roles = "ADMIN, SUPER")]
    [HttpPost("remove-claim/{id}")]
    public async Task<IActionResult> RemoveClaim(string id, [FromBody] ClaimDto claimDto)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound(ErrorResponse.SendMessage(Errors.NotFound404));
        }

        await _userManager.RemoveClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
        return Ok();
    }


    [HttpGet("verify-account")]
    public async Task<IActionResult> VerifyAccount([FromQuery(Name = "verificationCode")] string verificationCode)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.VerificationCode == verificationCode);

        if (user == null)
        {
            return NotFound();
        }

        if (user.EmailConfirmed)
        {
            return BadRequest(ErrorResponse.SendMessage(Errors.Repetition));
        }

        if (user.VerificationCode != verificationCode)
        {
            return BadRequest(ErrorResponse.SendMessage(Errors.InvalidCredentials));
        }

        user.EmailConfirmed = true;
        user.IsVerified = true;
        user.VerificationCode = null;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto passwordResetRequestDto)
    {
        var user = await _userManager.FindByEmailAsync(passwordResetRequestDto.Email);
        if (user == null)
        {
            return NotFound();
        }

        if ((bool)(!user.IsVerified)!)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(24);
        await _context.SaveChangesAsync();

        return Ok();

    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto passwordResetDto)
    {

        var user = await _context.Users.FirstOrDefaultAsync(a =>
            a.PasswordResetToken == passwordResetDto.PasswordResetToken);
        if (user == null)
        {
            return NotFound();
        }

        if (user.PasswordResetTokenExpiration == null || !(user.PasswordResetTokenExpiration > DateTime.UtcNow))
        {
            return NotFound();
        }

        var result =
            await _userManager.ResetPasswordAsync(user, passwordResetDto.PasswordResetToken, passwordResetDto.Password);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse.SendMessage(Errors.Server500));
        }

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiration = null;
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    private string GenerateJwtToken(ApplicationUser user, IEnumerable<Claim> claims)
    {

        try
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email),
                    "User email is required to generate a JWT token.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtOptions.Expires),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}


