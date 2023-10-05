using System.ComponentModel.DataAnnotations;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Users;

public class UserUpdateDto : BaseEntity
{
    private string _firstName;
    private string _lastName;

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

    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

}