using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;

namespace Market.Models.DTOS.Discounts;

public class DiscountUpsertDto
{

    [Required]
    [StringLength(Constants.MaxLength, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string Code { get; set; } = null!;

    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }

}