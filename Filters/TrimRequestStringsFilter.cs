using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Market.Filters;

[AttributeUsage(AttributeTargets.Property)]
public class TrimAttribute : Attribute
{
}

public class TrimRequestStringsAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            TrimProperties(argument);
        }

        base.OnActionExecuting(context);
    }

    private void TrimProperties(object? obj) // Make obj nullable
    {
        if (obj == null)
            return;

        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop => prop.CanRead && prop.CanWrite);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.GetCustomAttribute<TrimAttribute>() != null)
            {
                var value = (string?)property.GetValue(obj);
                if (value != null)
                {
                    property.SetValue(obj, value.Trim());
                }
            }
            else if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
            {
                TrimProperties(property.GetValue(obj));
            }
        }
    }
}