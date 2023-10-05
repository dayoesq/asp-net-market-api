using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Market.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

public class Product : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} name must be between {2} and {1} characters.")]
    public string Name { get; set; } = null!;
    [Required]
    [Column(TypeName = "decimal")]
    [Precision(18, 2)]
    public decimal Price { get; set; }
    [Required]
    [MinLength(Constants.MinLength)]
    public string Description { get; set; } = null!;
    public string? Identification { get; set; }
    public ICollection<string>? ImageUrls { get; set; }
    [Column(TypeName = "decimal")]
    [Precision(18, 2)]
    public decimal? Weight { get; set; }
    // Navigation properties start here
    public int? DiscountId { get; set; }
    [ForeignKey(nameof(DiscountId))]
    public Discount? Discount { get; set; }
    public int? SizeId { get; set; }
    [ForeignKey(nameof(SizeId))]
    public Size? Size { get; set; }
    public int? ColorId { get; set; }
    [ForeignKey(nameof(ColorId))]
    public Color? Color { get; set; }
    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))]
    public Brand Brand { get; set; } = null!;
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;
    public int ProductTypeId { get; set; }
    [ForeignKey(nameof(ProductTypeId))]
    public ProductType ProductType { get; set; } = null!;

}