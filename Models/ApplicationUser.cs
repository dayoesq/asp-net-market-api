using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;


[Index(nameof(Email), IsUnique = true)]
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string LastName { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public bool? IsVerified { get; set; }
    public string? VerificationCode { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}
