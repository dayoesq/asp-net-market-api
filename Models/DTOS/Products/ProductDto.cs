namespace Market.Models.DTOS;

public class ProductDto : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Description { get; set; } = null!;
    public string? Identification { get; set; }
    public ICollection<ProductImage>? Images { get; set; }
    public decimal? Weight { get; set; }
    // Navigation properties
    public int? DiscountId { get; set; }
    public int? SizeId { get; set; }
    public int? ColorId { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
    public int ProductTypeId { get; set; }


}