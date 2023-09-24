using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;

}