using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Code), IsUnique = true)]
public class Discount : BaseEntity
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