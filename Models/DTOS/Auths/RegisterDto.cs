using System.ComponentModel.DataAnnotations;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Auths;

public class RegisterDto
{
    private string _firstName;
    private string _lastName;
    private string _email;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string FirstName
    {
        get => _firstName;
        set => _firstName = Helper.ToTitleCase(value);
    }

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string LastName
    {
        get => _lastName;
        set => _lastName = Helper.ToTitleCase(value);
    }

    [Required]
    [EmailAddress]
    public string Email
    {
        get => _email;
        set => _email = value.ToLower();
    }

    [Required]
    [StringLength(60, MinimumLength = 10, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirmation { get; set; } = null!;

}