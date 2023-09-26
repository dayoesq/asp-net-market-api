using Market.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class ControllerOptionsSetup : IConfigureOptions<MvcOptions>
{

    public void Configure(MvcOptions options)
    {

        options.Filters.Add(typeof(TrimRequestStringsAttribute));
        options.Filters.Add(typeof(GlobalExceptionFilter));
        options.Filters.Add(typeof(BadRequestFilter));
        options.Filters.Add(typeof(ValidateImageAndVideoFilterAttribute));
    }
}
