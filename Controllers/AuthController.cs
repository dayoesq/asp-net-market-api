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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Market.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            
            return Created(nameof(Register), new SuccessResponseDto
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
                return Unauthorized(new { message = "Invalid credentials" });
            }
            
            if (!user.EmailConfirmed)
            {
                return Unauthorized(new { message = "User not verified" });
            }
            
            await _signInManager.SignInAsync(user, false);
            var token = GenerateJwtToken(user);
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { token, expiration = _configuration.GetValue<int>("JWT:ExpirationInMinutes") });
        }
        
        
        [HttpPost("create-role")]
        [Authorize(Roles = "admin, super")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var role = roleName.ToLower();
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (roleExists)
            {
                return Conflict(new { message = "Role already exists" });
            }

            var roleToAdd = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(roleToAdd);

            if (result.Succeeded)
            {
                return Created(nameof(CreateRole), new SuccessResponseDto
                {
                    Message = "Role created successfully"
                });
            }
            return BadRequest(new { message = "Role creation failed"});
        }


        [HttpPost("assign-role")]
        [Authorize(Roles = "admin, super")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentDto model)
        {
            var roleModel = model.Email.ToLower();
            var user = await _userManager.FindByEmailAsync(roleModel);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Except(existingRoles);

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (!result.Succeeded)
            {
                return StatusCode(500, new { message = "Failed to assign role"});
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new { message = "Roles assigned successfully", roles });
        }

        [HttpGet("verify-account")]
        public async Task<IActionResult> VerifyAccount([FromQuery(Name = "verificationCode")] string verificationCode)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.VerificationCode == verificationCode);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new { message = "Account is already verified" });
            }

            if (user.VerificationCode != verificationCode)
            {
                return BadRequest(new { message = "Invalid verification code" });
            }

            user.EmailConfirmed = true;
            user.IsVerified = true;
            user.VerificationCode = null;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? Ok(new { message = "Account verified successfully" }) :
                StatusCode(500, new { message = "Internal server error" });
        }

        [HttpPost("request-to-change-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto passwordResetRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(passwordResetRequestDto.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            if ((bool)(!user.IsVerified)!)
            {
                return BadRequest(new { message = "Unverified user" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();
        
            return Ok(new { message = "Password reset link sent" });
            
        }
        
        [HttpPost("password-reset")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto passwordResetDto)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(a => a.PasswordResetToken == passwordResetDto.PasswordResetToken);
            if (user == null)
            {
                return NotFound();
            }
            
            if (user.PasswordResetTokenExpiration == null || !(user.PasswordResetTokenExpiration > DateTime.UtcNow))
            {
                return NotFound();
            }
            
            var result = await _userManager.ResetPasswordAsync(user, passwordResetDto.PasswordResetToken, passwordResetDto.Password);
            
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Password reset failed" });
            }

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;
            await _context.SaveChangesAsync();
            
            return Ok();
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
            var expirationInMinutes = jwtConfig.GetValue<int>("ExpirationInMinutes");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }

