using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

[Index(nameof(Name), IsUnique = true)]
public class Category : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public  string Name { get; set; }
    public string? Description { get; set; }
   
}