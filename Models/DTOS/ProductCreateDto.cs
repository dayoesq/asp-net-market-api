using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class ProductCreateDto
{
    private string _name;
    private string _brand;

    [Required]
    [MinLength(2)]
    [MaxLength(50)]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);
    }

    public string Brand
    {
        get => _brand;
        set => _brand = Helper.ToTitleCase(value);
    }
    [Required]
    public decimal Price { get; set; }
    [Required]
    [MinLength(10)]
    public string Description { get; set; }
    public string? Identification { get; set; }
    public ICollection<ProductImage> Images { get; set; }
}