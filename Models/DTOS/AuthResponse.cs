namespace Market.Models.DTOS;

public class AuthResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}