using System.ComponentModel.DataAnnotations;

namespace Market.Models;

public class Size : BaseEntity
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    
} 