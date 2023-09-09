using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Market.Models;
using Market.OptionsSetup.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Market.Services.Jwt;

    public class JwtProvider : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;

        }
        public string GenerateToken(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email),
                    "User email is required to generate a JWT token.");
            }
            try
            {
                
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id),
                    new(ClaimTypes.Email, user.Email),
                };
                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(_jwtOptions.Expires),
                    Issuer = _jwtOptions.Issuer,
                    Audience = _jwtOptions.Audience,
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
    }
