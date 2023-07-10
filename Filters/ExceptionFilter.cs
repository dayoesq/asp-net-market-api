using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogInformation(context.Exception.Message);
        base.OnException(context);
    }
}