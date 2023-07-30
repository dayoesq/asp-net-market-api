using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;


public class BadRequestFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        var errors = new Dictionary<string, List<string>>();

        foreach (var key in context.ModelState.Keys)
        {
            var errorMessages = new List<string>();
            var modelErrorCollection = context.ModelState[key]?.Errors;
            if (modelErrorCollection != null) 
                errorMessages.AddRange(modelErrorCollection.Select(error => error.ErrorMessage));

            errors[key] = errorMessages;
        }

        context.Result = new ObjectResult(new { errors })
        {
            StatusCode = 400
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // This method is not used in this filter implementation.
    }
}