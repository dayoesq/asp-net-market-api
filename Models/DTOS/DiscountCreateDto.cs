using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class DiscountCreateDto
{
    
    [Required]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Code must be between {2} and {1} characters.")]
    public string? Code { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}