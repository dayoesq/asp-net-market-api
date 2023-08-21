using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;


public class TrimRequestStringsAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        foreach (var argument in actionContext.ActionArguments.Values)
        {
            if (argument == null)
                continue;

            var properties = argument.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var value = (string)property.GetValue(argument)!;
                property.SetValue(argument, value.Trim());
            }
        }

        base.OnActionExecuting(actionContext);
    }
}
