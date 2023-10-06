using System.ComponentModel.DataAnnotations;
using Market.Utils.Constants;

namespace Market.Models;

public class Size : BaseEntity
{
    public int Id { get; set; }
    [StringLength(Constants.MaxLength, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string ProductSize { get; set; } = null!;

} 