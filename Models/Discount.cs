using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Code), IsUnique = true)]
public class Discount : BaseEntity
{
    private string _code;
    public int Id { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The field '{0}' cannot be shorter than {1} characters")]
    [MaxLength(8, ErrorMessage = "The field '{0}' cannot be longer than {1} characters")]
    public string Code
    {
        get => _code;
        set => _code = value.ToUpper();
    }
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}