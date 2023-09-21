namespace Market.Models.DTOS;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}