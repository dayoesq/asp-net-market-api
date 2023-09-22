using Microsoft.AspNetCore.Mvc;

namespace Market.ApiBehaviours;

public static class BadRequestBehaviour
{
    public static void Parse(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var response = new Dictionary<string, List<string>>();

            foreach (var key in context.ModelState.Keys)
            {
                var modelErrorCollection = context.ModelState[key]?.Errors;
                if (modelErrorCollection != null)
                {
                    var errorMessages = modelErrorCollection.Select(error => error.ErrorMessage).ToList();
                    response[key] = errorMessages;
                }
            }

            return new BadRequestObjectResult(new { message = response });
        };
    }
}
