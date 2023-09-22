using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Claims;

public class ClaimDto
{
    private string _value;
    private string _type;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Type
    {
        get => _type;
        set => _type = value.ToLowerInvariant();
    }
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Value
    {
        get => _value;
        set => _value = value.ToLowerInvariant();
    }

}