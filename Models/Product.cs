namespace Market.Models;

public class Product
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Identification { get; set; }
    public ICollection<ProductImage> Images { get; set; }
}