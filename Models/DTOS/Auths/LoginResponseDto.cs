namespace Market.Models.DTOS.Auths;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}