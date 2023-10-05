using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS.Auths;
using Market.Models.DTOS.Claims;
using Market.OptionsSetup.Jwt;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Market.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly JwtOptionSettings _jwtOptions;
    private readonly ILogger<AuthController> _logger;


    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IMapper mapper, ApplicationDbContext context, IOptions<JwtOptionSettings> jwtOptions, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;


    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {

        try
        {
            if (!Regex.IsMatch(registerDto.FirstName, Constants.NamePattern) &&
                !Regex.IsMatch(registerDto.LastName, Constants.NamePattern))
            {
                return BadRequest(new { message = Errors.InvalidFormat + " name format" });
            }

            var user = _mapper.Map<ApplicationUser>(registerDto);
            var randomInt = Helper.GenerateRandomNumber(10);

            var accountVerification = new AccountVerification { Email = user.Email!, Code = (int)randomInt };

            await _userManager.CreateAsync(user, registerDto.Password);
            
            var claims = new List<Claim>
            {
                new(CustomClaimTypes.Sub, user.Id),
                new(CustomClaimTypes.Roles, Roles.User),
            };
            
            await _userManager.AddClaimsAsync(user, claims);
            _context.AccountVerifications.Add(accountVerification);
            await _context.SaveChangesAsync();
            // Send mail token to user.
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto auth)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(auth.Email);
        
            if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
            {
                return Unauthorized(new { message = Errors.InvalidCredentials });
            }
        
            if (!user.EmailConfirmed)
            {
                return BadRequest(new { message = Errors.UnverifiedAccount });
            }

            await _signInManager.PasswordSignInAsync(user, auth.Password, isPersistent: false, lockoutOnFailure: false);
            var token = await GenerateJwtToken(user);
            user.LastLogin = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Ok(new { Token = token });

        }
        catch (Exception e)
        {
            _logger.LogCritical($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("logout")]
    public IActionResult Logout()
    {

        return Ok(new { message = ResponseMessage.Success });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
    [HttpPost("assign-claim/{id}")]
    public async Task<IActionResult> AssignClaim(string id, [FromBody] ClaimUpsertDto claimDto)
    {

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
       
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var claimExists = existingClaims.Any(c => c.Type == claimDto.Type && c.Value == claimDto.Value);
        if(claimExists) return Conflict(new { message = Errors.Conflict409 });
        await _userManager.AddClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
        
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Operation.SuperAdmin)]
    [HttpDelete("remove-claim/{id}")]
    public async Task<IActionResult> RemoveClaim(string id, [FromBody] ClaimUpsertDto claimDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound(new { message = Errors.NotFound404 });
        await _userManager.RemoveClaimAsync(user, new Claim(claimDto.Type, claimDto.Value));
        
        return Ok();
    }

    [HttpGet("verify-account")]
    public async Task<IActionResult> VerifyAccount([FromQuery(Name = "verificationCode")] int verificationCode)
    {
        try
        {
            var token = await _context.AccountVerifications.FirstOrDefaultAsync(a => a.Code == verificationCode);
            
            var user = _context.Users.FirstOrDefaultAsync(u => token != null && u.Email == token.Email);
            var mappedUser = _mapper.Map<ApplicationUser>(user);
            if (mappedUser.EmailConfirmed) return BadRequest(new { message = Errors.Repetition });
            mappedUser.EmailConfirmed = true;
            await _userManager.UpdateAsync(mappedUser);
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = Errors.Server500 });
        }
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto passwordResetRequestDto)
    {
        var user = await _userManager.FindByEmailAsync(passwordResetRequestDto.Email);
        if (user == null) return NotFound();
        if (!user.EmailConfirmed) return StatusCode(StatusCodes.Status401Unauthorized);
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var existingToken = await _context.PasswordResetTokens.FirstOrDefaultAsync(t => t.Email == user.Email);
        if (existingToken != null) _context.Remove(existingToken);
        
        await _context.SaveChangesAsync();
        
        var passwordResetToken = new PasswordResetToken
        {
            Email = user.Email!, Token = token, ExpiresAt = DateTime.UtcNow.AddHours(1), CreatedAt = DateTime.UtcNow
        };
        
        _context.PasswordResetTokens.Add(passwordResetToken);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto passwordResetDto)
    {
        try
        {
            var token = await _context.PasswordResetTokens.FirstOrDefaultAsync(t =>
                t.Token == passwordResetDto.PasswordResetToken);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == token!.Email);
            if (user == null) return NotFound();

            if (token!.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
                return BadRequest(new { message = Errors.ExpiredToken });
            
            await _userManager.ResetPasswordAsync(user, passwordResetDto.PasswordResetToken,
                    passwordResetDto.Password);
            _context.PasswordResetTokens.Remove(token);
            
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError($"{Errors.Server500}-{e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = Errors.Server500 });
        }
    }

    private async Task<IEnumerable<Claim>> AddDefaultClaimsToUser(ApplicationUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        if (claims.Count > 0) return claims;
        claims = new List<Claim>
        {
            new(CustomClaimTypes.Sub, user.Id),
            new(CustomClaimTypes.Roles, Roles.User),
        };

        await _userManager.AddClaimsAsync(user, claims);
        return claims;
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {

        try
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email),
                    "User email is required to generate a JWT token.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var claims = await AddDefaultClaimsToUser(user);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.Expires),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}


