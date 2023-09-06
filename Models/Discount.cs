using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Code), IsUnique = true)]
public class Discount : BaseEntity
{
    private string _code;
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Code must be between '{2}' and {1} characters.")]
    public string Code
    {
        get => _code;
        set => _code = value.ToUpper();
    }
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }
    
}