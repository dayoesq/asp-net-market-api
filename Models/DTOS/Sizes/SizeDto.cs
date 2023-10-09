namespace Market.Models.DTOS.Sizes;

public class SizeDto : BaseEntity
{
    public int Id { get; set; }
    public string ProductSize { get; set; } = null!;
}