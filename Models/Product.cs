using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Market.Models;

public class Product : BaseEntity
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(20)]
    public string Name { get; set; }
    
    public string Brand { get; set; }
    [Required]
    [Column(TypeName = "decimal")]
    [Precision(18, 2)]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? Identification { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public int DiscountId { get; set; }
    [ForeignKey(nameof(DiscountId))]
    public Discount Discount { get; set; }
    public ICollection<Category> Categories { get; set; }
    public int SizeId { get; set; }
    [ForeignKey(nameof(SizeId))]
    public Size? Size { get; set; }
    public int ColorId { get; set; }
    [ForeignKey(nameof(ColorId))]
    public Color? Color { get; set; }
   
}