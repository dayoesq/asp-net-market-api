using Microsoft.AspNetCore.Authorization;

namespace Market.Permissions;

public class Permission : IAuthorizationRequirement
{
    public Permission(string role) => Role = role;

    public string Role { get; }
}