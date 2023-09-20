using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace Market.Models.DTOS;

public class ProductCreateDto
{
    private string _name;
    private string _brand;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} name must be between {2} and {1} characters.")]
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
    [Column(TypeName = "decimal")]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; } = null!;
    public string? Identification { get; set; }
    public ICollection<ProductImage>? Images { get; set; }
    public int? DiscountId { get; set; }
    [ForeignKey(nameof(DiscountId))]
    public Discount? Discount { get; set; }
    [Required]
    public ICollection<Category>? Categories { get; set; }

}