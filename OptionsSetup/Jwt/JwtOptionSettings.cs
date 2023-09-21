namespace Market.OptionsSetup.Jwt;

public class JwtOptionSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int Expires { get; init; }
}