using System.ComponentModel.DataAnnotations;
using Market.Utils;

namespace Market.Models.DTOS;

public class ColorCreateDto
{
    private string _name;
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);
    }
}