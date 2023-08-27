using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class DiscountUpdateDto
{
    
    [MinLength(4, ErrorMessage = "The field '{0}' cannot be shorter than {1} characters")]
    [MaxLength(8, ErrorMessage = "The field '{0}' cannot be longer than {1} characters")]
    public string Code { get; set;  }
    [Range(0, 100)]
    public int Percentage { get; set; }
}