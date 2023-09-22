using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS.Categories;

public class CategoryCreateDto
{
    private string _name;
    
    
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} must be between {2} and {1} characters.")]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);

    }
    public string? Description { get; set; }
    
}