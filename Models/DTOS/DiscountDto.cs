namespace Market.Models.DTOS;

public class DiscountDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int Percentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}