using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class DiscountCreateDto
{
    
    public int Id { get; set; }
    
    [Required]
    [MinLength(4)]
    [MaxLength(8)]
    public string Code { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}