using System.ComponentModel.DataAnnotations;
using Market.Utils;
using Market.Utils.Constants;

namespace Market.Models.DTOS.ProductTypes;

public class ProductTypeUpsertDto
{
    private string _type;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Type
    {
        get => _type;
        set => _type = Helper.ToTitleCase(value);
    }
}