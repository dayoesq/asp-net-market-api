using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class DiscountCreateDto
{
    private string _name;
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(20)]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);

    }
    
    [Required]
    [MinLength(4)]
    [MaxLength(8)]
    public string? Code { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}