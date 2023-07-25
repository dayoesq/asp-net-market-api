using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Market.Models;
using Market.Models.DTOS;
using Market.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Market.Controllers;

[ApiController]
[Route(Constants.apiAccountPrefix)]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> SignUp([FromBody] RegisterCredential register )
    {
        var user = new ApplicationUser
        {
            FirstName = register.FirstName,
            LastName = register.LastName,
            Password = register.Password,
            UserName = register.Email, 
            Email = register.Email,
            
        };
        var result = await _userManager.CreateAsync(user, register.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return BuildToken("SIGNUP");

    }

    private AuthResponse BuildToken(string mode)
    {
        var register = new RegisterCredential();
        var login = new AuthCredential();

        var claims = mode.ToUpper() switch
        {
            "SIGNUP" => new List<Claim> { new("email", register.Email) },
            "LOGIN" => new List<Claim> { new("email", login.Email) },
            _ => null
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Secret")!));
        var details = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddYears(1);
        var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
            signingCredentials: details);
        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };

    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthCredential auth)
    {
        var result = await _signInManager
            .PasswordSignInAsync(auth.Email, auth.Password, false, false);
        if (!result.Succeeded) return BadRequest("Invalid credentials");
        return BuildToken("LOGIN");
    }
}