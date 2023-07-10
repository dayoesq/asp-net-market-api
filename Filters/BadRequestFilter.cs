using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Market.Filters;

public class BadRequestFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if(context.Result is not IStatusCodeActionResult result) return;

        var statusCode = result.StatusCode;
        if (statusCode == 400)
        {
            var response = new List<string?>();
            var badRequestObjectResult = context.Result as BadRequestObjectResult;
            if (badRequestObjectResult?.Value is string)
            {
                response.Add(badRequestObjectResult.Value.ToString());
                
            }
            else
            {
                foreach (var key in context.ModelState.Keys)
                {
                    var modelErrorCollection = context.ModelState[key]?.Errors;
                    if (modelErrorCollection != null)
                        foreach (var error in modelErrorCollection)
                        {
                            response.Add($"{key}: {error.ErrorMessage}");
                        }
                }
            }
            context.Result = new BadRequestObjectResult(response);
        }
        
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }
}