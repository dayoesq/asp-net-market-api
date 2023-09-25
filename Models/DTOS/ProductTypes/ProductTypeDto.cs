namespace Market.Models.DTOS.ProductTypes;

public class ProductTypeDto : BaseEntity
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
}