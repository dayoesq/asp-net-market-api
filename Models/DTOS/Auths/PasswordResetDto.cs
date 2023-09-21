using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class PasswordResetDto
{
    [Required]
    public string PasswordResetToken { get; set; } = null!;
    [Required]
    [MaxLength(60)]
    public string Password { get; set; } = null!;

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string PasswordConfirmation { get; set; } = null!;
}