using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class AccountRoleDto
{
    private string _role;

    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Role must be between {1} and {2} characters.")]
    public string Role
    {
        get => _role;
        set => _role = Helper.ToUpper(value);
    } 
}