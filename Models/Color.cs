using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Name), IsUnique = true)]
public class Color : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [MinLength(3)]
    public string? Name { get; set; }
    
}