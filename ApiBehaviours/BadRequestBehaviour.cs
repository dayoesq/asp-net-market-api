using Microsoft.AspNetCore.Mvc;

namespace Market.ApiBehaviours;

public static class BadRequestBehaviour
{
    public static void Parse(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var response = new List<string?>();
            foreach (var key in context.ModelState.Keys)
            {
                var modelErrorCollection = context.ModelState[key]?.Errors;
                if (modelErrorCollection != null)
                    foreach (var error in modelErrorCollection)
                    {
                        response.Add($"{key}: {error.ErrorMessage}");
                    }
            }

            return new BadRequestObjectResult(response);
        };
    }
}