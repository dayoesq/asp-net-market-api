using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models;

public class ProductImage : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [MaxLength(150)]
    public string? Name { get; set; }
    [Required]
    public string? Path { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Size { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; init; }
    
}