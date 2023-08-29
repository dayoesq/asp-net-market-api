using Market.Models;

namespace Market.Services.Jwt;

public interface IJwtService
{ 
    public string GenerateToken(ApplicationUser user);
}