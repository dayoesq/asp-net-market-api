using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;

namespace Market.Models.DTOS.ProductImages;

public class ImageUpsertDto
{
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Url { get; set; } = null!;
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

}