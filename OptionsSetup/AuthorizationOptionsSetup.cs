using Market.Utils.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class AuthorizationOptionsSetup : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {

        options.AddPolicy(Roles.Admin, policy => policy.RequireClaim(CustomClaimTypes.Roles, Roles.Admin));
        options.AddPolicy(Roles.Super, policy => policy.RequireClaim(CustomClaimTypes.Roles, Roles.Super));
        options.AddPolicy(Roles.Management, policy => policy.RequireClaim(CustomClaimTypes.Roles, Roles.Management));
        options.AddPolicy(Roles.User, policy => policy.RequireClaim(CustomClaimTypes.Roles, Roles.User));
    }
}
