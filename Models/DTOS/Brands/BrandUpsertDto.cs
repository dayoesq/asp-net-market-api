using System.ComponentModel.DataAnnotations;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Brands;

public class BrandUpsertDto
{
    private string _name;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);
    }
}