using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
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

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var userExists = await _userManager.FindByEmailAsync(register.Email);
            if (userExists != null)
            {
                return Conflict(new { message = "User with the provided email already exists." });
            }

            var user = _mapper.Map<ApplicationUser>(register);
            user.UserName = register.Email; // Set the UserName to be the same as Email

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var verificationCode = StringGenerator.GenerateRandomNumber(8);
            user.VerificationCode = verificationCode;

            // Save the user to the database
            var saveResult = await _userManager.UpdateAsync(user);

            if (!saveResult.Succeeded)
            {
                return BadRequest(saveResult.Errors);
            }

            // Send the verification email
            //await _emailService.SendVerificationEmail(user.Email, verificationCode);

            var responseDto = new ResponseDto
            {
                Message = "Registration successful. A verification code has been sent to your email."
            };

            return Created("api/auth/register", responseDto);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto auth)
        {
            var user = await _userManager.FindByEmailAsync(auth.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            await _signInManager.SignInAsync(user, false);

            // There should be a check for user verified status.

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
            var jwtConfig = _configuration.GetSection("JWT");
            var secret = jwtConfig.GetValue<string>("Secret");
            var issuer = jwtConfig.GetValue<string>("Issuer");
            var audience = jwtConfig.GetValue<string>("Audience");
            var expirationInMinutes = jwtConfig.GetValue<int>("ExpirationInMonths");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName!) // Add UserName to claims
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMonths(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

