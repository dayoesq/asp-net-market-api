using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;

public class BadRequestFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var modelErrors = new Dictionary<string, List<string>>();

            foreach (var key in context.ModelState.Keys)
            {
                var errorMessages = context.ModelState[key]!.Errors.Select(error => error.ErrorMessage).ToList();
                modelErrors[key] = errorMessages;
            }

            context.Result = new ObjectResult(new { errors = modelErrors })
            {
                StatusCode = 400
            };
        }

        if (!context.ActionArguments.TryGetValue("identityResult", out var identityResultObj)
            || identityResultObj is not IEnumerable<IdentityError> identityErrors) return;

        var identityErrorsDict = new Dictionary<string, List<string>>();

        foreach (var identityError in identityErrors)
        {
            var errorMessages = new List<string> { identityError.Description };
            identityErrorsDict["IdentityErrors"] = errorMessages;
        }

        context.Result = new ObjectResult(new { errors = identityErrorsDict })
        {
            StatusCode = 400
        };

        // Handle additional HTTP statuses with customized messages
        HandleCustomStatusCodes(context);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // This method is not used in this filter implementation.
    }

    private void HandleCustomStatusCodes(ActionExecutingContext context)
    {
        // Check for specific cases and return custom responses
        var statusCode = context.HttpContext.Response.StatusCode;
        switch (statusCode)
        {
            case 401:
                context.Result = new ObjectResult(new { message = "Unauthorized." })
                {
                    StatusCode = 401
                };
                break;
            case 403:
                context.Result = new ObjectResult(new { message = "Forbidden." })
                {
                    StatusCode = 403
                };
                break;
            case 404:
                context.Result = new ObjectResult(new { message = "Not found." })
                {
                    StatusCode = 404
                };
                break;
            case 429:
                context.Result = new ObjectResult(new { message = "Too many requests." })
                {
                    StatusCode = 429
                };
                break;
            case 422:
                context.Result = new ObjectResult(new { message = "Unprocessable entity." })
                {
                    StatusCode = 422
                };
                break;
                // Add more custom status code cases as needed
        }
    }
}
