using Market.Utils;

namespace Market.Models.DTOS;

public class CategoryUpdateDto
{
    private string _name;

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set => _name = Helper.ToTitleCase(value);

    }
    public string? Description { get; set; }

}