using System.Security.Claims;
using Market.Utils.Constants;

public static class HttpContextExtensions
{
    public static ClaimsPrincipal AuthUser(this HttpContext httpContext)
    {
        return httpContext.User;
    }
    public static string Email(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(CustomClaimTypes.Email)!;
    }

    public static ClaimsPrincipal Id(this HttpContext httpContext)
    {
        return httpContext.User;
    }

    public static IEnumerable<string> Roles(this HttpContext httpContext)
    {
        yield return httpContext.User.FindFirstValue(CustomClaimTypes.Roles)!;
    }
}
