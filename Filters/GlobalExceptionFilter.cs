using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred.");

        context.Result = context.Exception switch
        {
            NotFoundException notFoundException => new NotFoundObjectResult(notFoundException.Message) { StatusCode = 404 },
            ValidationException validationException => new BadRequestObjectResult(validationException.Message)
            {
                StatusCode = 400
            },
            ConflictException conflictException => new ConflictObjectResult(conflictException.Message)
            {
                StatusCode = 409
            },
            _ => new ObjectResult("An error occurred.") { StatusCode = 500 }
        };

        context.ExceptionHandled = true;
    }
}

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message) { }
}

public abstract class ValidationException : Exception
{
    protected ValidationException(string message) : base(message) { }
}

public abstract class ConflictException : Exception
{
    protected ConflictException(string message) : base(message) { }
}
