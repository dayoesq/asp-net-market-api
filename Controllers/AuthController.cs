using System.Text.RegularExpressions;
using AutoMapper;
using Market.DataContext;
using Market.Models;
using Market.Models.DTOS;
using Market.Services.Jwt;
using Market.Utils;
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
        
            if (!Regex.IsMatch(registerDto.FirstName, Constants.NAME_PATTERN) && !Regex.IsMatch(registerDto.LastName, Constants.NAME_PATTERN))
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
            
            return Created(nameof(Register), registerDto);
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto auth)
        {
            var user = await _userManager.FindByEmailAsync(auth.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, auth.Password))
            {
                return Unauthorized(new ErrorResponse(Errors.UnAuthorized401));
            }
            
            if (!user.EmailConfirmed)
            {
                return Unauthorized(new ErrorResponse(Errors.UnAuthorized401));
            }
            
            await _signInManager.SignInAsync(user, false);
            var token = _jwtService.GenerateToken(user);
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { token, expiration = _configuration.GetValue<int>("JWT:ExpirationInMinutes") });
        }
        
        
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var role = roleName.ToLower();
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (roleExists)
            {
                return Conflict(new ErrorResponse(Errors.Conflict409));
            }

            var roleToAdd = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(roleToAdd);

            return result.Succeeded ? Created(nameof(CreateRole), role) : StatusCode(500, new ErrorResponse(Errors.Server500));
        }


        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentDto model)
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
                return StatusCode(500, new ErrorResponse(Errors.Server500));
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
                return NotFound(new ErrorResponse(Errors.NotFound404));
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
                StatusCode(500, new ErrorResponse(Errors.Server500));
        }

        [HttpPost("request-to-change-password")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto passwordResetRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(passwordResetRequestDto.Email);
            if (user == null)
            {
                return NotFound(new ErrorResponse(Errors.NotFound404));
            }
            
            if ((bool)(!user.IsVerified)!)
            {
                return Unauthorized(new ErrorResponse(Errors.UnAuthorized401));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();
        
            return Ok();
            
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
                return StatusCode(500, new ErrorResponse(Errors.Server500));
            }

            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;
            await _context.SaveChangesAsync();
            
            return Ok();
        }
        
        
    }

