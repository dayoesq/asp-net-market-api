using Microsoft.Extensions.Options;

namespace Market.OptionsSetup.Jwt;

public class JwtOptionsSetup : IConfigureOptions<JwtOptionSettings>
{
    private const string SectionName = "Jwt";

    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptionSettings options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}