namespace Market.Models.DTOS.ProductImages;

public class ProductImageDto : BaseEntity
{

    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public int ProductId { get; set; }

}