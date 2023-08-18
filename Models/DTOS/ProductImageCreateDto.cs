using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Market.Models.DTOS;

public class ProductImageCreateDto
{
    [Required]
    [MaxLength(150)]
    public string? Name { get; set; }
    [Required]
    public string? Path { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; init; }
   
}