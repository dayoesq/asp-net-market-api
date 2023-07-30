using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class LoginDto
{
    [Required]
    public string Email { get; set;  }
    [Required]
    public string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(60)]
    [MinLength(10)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirmation { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}