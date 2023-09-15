using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class ClaimDto
{
    private string _value;
    private string _type;

    [Required]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Type must be between {1} and {2} characters.")]
    public string Type
    {
        get => _type;
        set => _type = value.ToLowerInvariant();
    }
    [Required]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Value must be between {1} and {2} characters.")]
    public string Value
    {
        get => _value;
        set => _value = value.ToLowerInvariant();
    } 
    
}