using System.ComponentModel.DataAnnotations;

namespace Market.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public  string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}