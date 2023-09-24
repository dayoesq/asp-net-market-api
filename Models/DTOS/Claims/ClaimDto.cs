namespace Market.Models.DTOS.Claims;

public class ClaimDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}