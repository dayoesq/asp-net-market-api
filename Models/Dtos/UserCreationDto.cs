using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models.Dtos;

public class UserCreationDto
{
    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    [DisplayName("First Name")]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(25)]
    [MinLength(2)]
    [DisplayName("Last Name")]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MaxLength(60)]
    [MinLength(10)]
    public string Password { get; set; }
    [Compare("Password")]
    [DisplayName("Password Confirmation")]
    public string PasswordConfirmation { get; set; }
    public string? Address { get; set; }
    public string? Telephone { get; set; }
    [DisplayName("IP Address")]
    public string? IPAddress { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}