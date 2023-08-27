using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class DiscountCreateDto
{
    
    [Required]
    [MinLength(4, ErrorMessage = "The field '{0}' cannot be shorter than {1} characters")]
    [MaxLength(8, ErrorMessage = "The field '{0}' cannot be longer than {1} characters")]
    public string Code { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}