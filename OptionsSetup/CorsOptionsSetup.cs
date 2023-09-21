using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;


public class CorsOptionsSetup : IConfigureOptions<CorsOptions>
{
    private readonly IConfiguration _configuration;

    public CorsOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(CorsOptions options)
    {

        var clientBaseUrl = _configuration.GetValue<string>("ClientBaseUrl");
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(clientBaseUrl!).AllowAnyMethod().AllowAnyHeader();
        });
    }
}
