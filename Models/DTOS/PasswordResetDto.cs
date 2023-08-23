using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class PasswordResetDto
{
    [Required]
    public string PasswordResetToken { get; set; }
    [Required]
    [MaxLength(60)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirmation { get; set; }
}