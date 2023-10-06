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
            ValidationException validationException => new BadRequestObjectResult(validationException.Message) { StatusCode = 400 },
            ConflictException conflictException => new ConflictObjectResult(conflictException.Message) { StatusCode = 409 },
            UnauthorizedAccessException unauthorizedException => new ObjectResult(unauthorizedException.Message) { StatusCode = 401 },
            ForbiddenException forbiddenException => new ObjectResult(forbiddenException.Message) { StatusCode = 403 },
            TooManyRequestsException tooManyRequestsException => new ObjectResult(tooManyRequestsException.Message) { StatusCode = 429 },
            MovedPermanentlyException movedPermanentlyException => new ObjectResult(movedPermanentlyException.Message) { StatusCode = 301 },
            
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

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message) : base(message) { }
}

public class MovedPermanentlyException : Exception
{
    public MovedPermanentlyException(string message) : base(message) { }
}
