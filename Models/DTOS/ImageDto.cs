using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models.DTOS;

public class ImageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public int Size { get; set; }
    [ForeignKey(nameof(Product))] 
    public int ProductId { get; set; }
    public Product Product { get; set; }
}