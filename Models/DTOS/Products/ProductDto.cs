using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models.DTOS;

public class ProductDto : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal Price { get; set; }
    public string Description { get; set; } = null!;
    public string? Identification { get; set; } = null!;
    public ICollection<ProductImage>? Images { get; set; }
    public ICollection<Category>? Categories { get; set; }
    public int? DiscountId { get; set; }
    [ForeignKey(nameof(DiscountId))]
    public Discount? Discount { get; set; }

}