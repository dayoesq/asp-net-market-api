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
            var errors = new Dictionary<string, List<string>>();

            foreach (var key in context.ModelState.Keys)
            {
                var errorMessages = context.ModelState[key]!.Errors.Select(error => error.ErrorMessage).ToList();

                errors[key] = errorMessages;
            }

            context.Result = new ObjectResult(new { errors })
            {
                StatusCode = 400
            };
        }

        if (!context.ActionArguments.TryGetValue("identityResult", out var identityResultObj)
            || identityResultObj is not IEnumerable<IdentityError> identityErrors) return;
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var identityError in identityErrors)
            {
                var errorMessages = new List<string> { identityError.Description };
                errors["IdentityErrors"] = errorMessages;
            }

            context.Result = new ObjectResult(new { errors })
            {
                StatusCode = 400
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // This method is not used in this filter implementation.
    }
}
