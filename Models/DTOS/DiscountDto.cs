namespace Market.Models.DTOS;

public class DiscountDto : BaseEntity
{
    public int Id { get; set; }
    public string Code { get; set; }
    public int Percentage { get; set; }
    
}