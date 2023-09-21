using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models;

public class ProductCategory
{
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
}