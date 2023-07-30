using Microsoft.AspNetCore.Mvc;

namespace Market.ApiBehaviours;

public static class BadRequestBehaviour
{
    public static void Parse(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var response = (
                from key in context.ModelState.Keys let modelErrorCollection = context.ModelState[key]?.Errors 
                where modelErrorCollection != null from error 
                    in modelErrorCollection select $"{key}: {error.ErrorMessage}").ToList();

            return new BadRequestObjectResult(response);
        };
    }
}