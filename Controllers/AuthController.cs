using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Market.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Market.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
        
            if (!Regex.IsMatch(registerDto.FirstName, Constants.NAME_PATTERN) && !Regex.IsMatch(registerDto.LastName, Constants.NAME_PATTERN))
            {
                return BadRequest("Invalid name format");
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
            
            await _context.SaveChangesAsync();

            // Send the verification email
            //await _emailService.SendVerificationEmail(user.Email, verificationCode);
            
            return Created("api/auth/register", new SuccessResponseDto
            {
                Message = "Registration successful"
            });
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto auth)
        {
            var user = await _userManager.FindByEmailAsync(auth.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            if (!user.EmailConfirmed)
            {
                return Unauthorized(new { message = "Not verified." });
            }
            
            await _signInManager.SignInAsync(user, false);
            var token = GenerateJwtToken(user);

            return Ok(new { token, expiration = _configuration.GetValue<int>("JWT:ExpirationInMonths") });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "admin, super")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Except(existingRoles);

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new { message = "Roles assigned successfully.", roles });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            if (user.Email == null)
            {
                throw new ArgumentNullException(nameof(user.Email), "User email is required to generate a JWT token");
            }

            var jwtConfig = _configuration.GetSection("JWT");
            var secret = jwtConfig.GetValue<string>("Secret");
            var issuer = jwtConfig.GetValue<string>("Issuer");
            var audience = jwtConfig.GetValue<string>("Audience");
            var expirationInMonths = jwtConfig.GetValue<int>("ExpirationInMonths");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.UserName!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMonths(expirationInMonths),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }

