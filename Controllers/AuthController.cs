using System.Text.RegularExpressions;
using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Market.Services.Jwt;
using Market.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly  IJwtService _jwtService;
        

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IMapper mapper, ApplicationDbContext context, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
        
            if (!Regex.IsMatch(registerDto.FirstName, Constants.NamePattern) && !Regex.IsMatch(registerDto.LastName, Constants.NamePattern))
            {
                return BadRequest(new ErrorResponse(Errors.InvalidFormat + " name format"));
            }

            var currentUser = _userManager.FindByEmailAsync(registerDto.Email);
            if (currentUser.Result?.Email?.ToUpperInvariant() == registerDto.Email.ToUpperInvariant())
            {
                return Conflict(new ErrorResponse(Errors.Conflict409));
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
            
            return StatusCode(StatusCodes.Status201Created);
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto auth)
        {
            var user = await _userManager.FindByEmailAsync(auth.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
            {
                return Unauthorized(new ErrorResponse(Errors.InvalidCredentials));
            }
            
            if (!user.EmailConfirmed)
            {
                return BadRequest(new ErrorResponse(Errors.UnverifiedAccount));
            }
            
            await _signInManager.SignInAsync(user, false);
            var token = _jwtService.GenerateToken(user);
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { token, expiration = _configuration.GetValue<int>("JWT:ExpirationInMinutes") });
        }
        
        
        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        
        [Authorize(Roles = "ADMIN, SUPER")]
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] AccountRoleDto roleDto)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleDto.Role);

            if (roleExists)
            {
                return Conflict(new ErrorResponse(Errors.Conflict409));
            }

            var roleToAdd = new IdentityRole(roleDto.Role);
            var result = await _roleManager.CreateAsync(roleToAdd);

            return result.Succeeded ? StatusCode(StatusCodes.Status201Created): 
                StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize(Roles = "ADMIN, SUPER")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto model)
        {
            var roleModel = model.Email.ToLower();
            var user = await _userManager.FindByEmailAsync(roleModel);

            if (user == null)
            {
                return NotFound(new ErrorResponse(Errors.NotFound404));
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Except(existingRoles);

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (!result.Succeeded)
            {
                return  StatusCode(StatusCodes.Status500InternalServerError);
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
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
                return BadRequest(new ErrorResponse(Errors.Repetition));
            }

            if (user.VerificationCode != verificationCode)
            {
                return BadRequest(new ErrorResponse(Errors.InvalidCredentials));
            }

            user.EmailConfirmed = true;
            user.IsVerified = true;
            user.VerificationCode = null;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded ? Ok() :
                StatusCode(StatusCodes.Status500InternalServerError);
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
                return  StatusCode(StatusCodes.Status401Unauthorized);
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
                return  StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(Errors.Server500));
            }

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        
    }

