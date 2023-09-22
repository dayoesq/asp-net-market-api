using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS.Auths;

public class PasswordResetRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}