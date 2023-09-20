using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Code), IsUnique = true)]
public class Discount : BaseEntity
{
    private string _code;
    public int Id { get; set; }

    [Required]
    [StringLength(Constants.MaxLength, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string Code
    {
        get => _code;
        set => _code = value.ToUpper();
    }
    [Required]
    [Range(0, 100)]
    public int Percentage { get; set; }

}