namespace Market.Models.DTOS;

public class JwtTokenDescriptorsDto
{
    public string Issuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int Expiration { get; set; }
    public string Audience { get; set; } = string.Empty;
}