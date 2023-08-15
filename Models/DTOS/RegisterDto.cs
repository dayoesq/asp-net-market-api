using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class RegisterDto
{
    private string _firstName;
    private string _lastName;
    
    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    public string FirstName
    {
        get => _firstName;
        set => _firstName = Helper.ToTitleCase(value);
    }

    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    public string LastName
    {
        get => _lastName;
        set => _lastName = Helper.ToTitleCase(value);
    }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MaxLength(60)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirmation { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}