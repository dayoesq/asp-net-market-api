using System.ComponentModel.DataAnnotations;
using Market.Filters;
using Market.Utils.Constants;

namespace Market.Models;

public class Brand : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [Trim]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name { get; set; } = null!;

}