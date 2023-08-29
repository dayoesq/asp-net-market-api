using Microsoft.Extensions.Options;

namespace Market.OptionsSetup;

public class JwtOptionsSetup : IConfigureOptions<JwtOptionsSetup>
{

    private const string Jwt = "JWT";
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(JwtOptionsSetup optionsSetup)
    {
        _configuration.GetSection(Jwt).Bind(optionsSetup);
    }
}