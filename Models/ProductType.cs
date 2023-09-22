using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;


namespace Market.Models;

public class ProductType : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} name must be between {2} and {1} characters.")]
    public string Type { get; set; } = null!;

}