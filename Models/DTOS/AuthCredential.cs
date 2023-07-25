using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class AuthCredential
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

public class RegisterCredential
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
    [Compare("Password")]
    public string PasswordConfirmation { get; set; }
}