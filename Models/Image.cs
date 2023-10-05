using System.ComponentModel.DataAnnotations;

namespace Market.Models;

public class Image : BaseEntity
{
    public int Id { get; set; }
    [Required]
    public string Url { get; set; } = null!;

}