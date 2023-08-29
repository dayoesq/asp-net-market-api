namespace Market.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred");

        context.Result = context.Exception switch
        {
            NotFoundException => new NotFoundObjectResult("Resource not found") { StatusCode = 404 },
            ValidationException validationException => new BadRequestObjectResult(validationException.ValidationErrors)
            {
                StatusCode = 400
            },
            _ => new ObjectResult("An error occurred") { StatusCode = 500 }
        };

        context.ExceptionHandled = true;
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }

    public string[] ValidationErrors { get; set; }
}
