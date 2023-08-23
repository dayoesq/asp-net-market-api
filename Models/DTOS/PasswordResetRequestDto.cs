using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class PasswordResetRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}