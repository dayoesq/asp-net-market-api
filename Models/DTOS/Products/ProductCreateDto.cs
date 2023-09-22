using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Market.Utils;
using Market.Utils.Constants;
using Microsoft.EntityFrameworkCore;

namespace Market.Models.DTOS;

public class ProductCreateDto
{
    private string _name;

    [Required]
    [StringLength(Constants.MaxLength, MinimumLength = Constants.MinLength, ErrorMessage = "{0} name must be between {2} and {1} characters.")]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);
    }
    [Required]
    [Column(TypeName = "decimal")]
    [Precision(18, 2)]
    public decimal Price { get; set; }
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public int ProductTypeId { get; set; }
    [ForeignKey(nameof(ProductTypeId))]
    public ProductType ProductType { get; set; } = null!;
    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))]
    public Brand Brand { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = null!;
    public ICollection<ProductCategory>? ProductCategories { get; set; }

}