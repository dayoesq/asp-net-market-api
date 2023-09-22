using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS.Sizes;

public class SizeCreateDto
{
    private string _size;
    
    public int Id { get; set; }

    [Required]
    public string Name
    {
        get => _size;
        set => _size = Helper.ToUpper(value);
    }
}