using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class LoginDto
{
    [Required]
    public string Email { get; set;  } = null!;
    [Required]
    public string Password { get; set; } = null!;
}