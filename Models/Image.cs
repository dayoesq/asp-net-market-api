using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models;

public class Image : BaseEntity
{
    public int Id { get; set; }
    [Required]
    public string Url { get; set; } = null!;
    public int? ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

}