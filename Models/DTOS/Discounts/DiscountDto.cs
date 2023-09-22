namespace Market.Models.DTOS.Discounts;

public class DiscountDto : BaseEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public int Percentage { get; set; }

}