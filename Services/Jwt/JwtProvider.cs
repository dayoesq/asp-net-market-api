using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Market.Models;
using Market.Models.DTOS;
using Microsoft.IdentityModel.Tokens;

namespace Market.Services.Jwt;

    public class JwtProvider : IJwtService
    {
        private const string Jwt = "JWT";
        private const string SecretKey = "SecretKey";
        private const string Audience = "Audience";
        private const string ExpirationInMinutes = "ExpirationInMinutes";
        private const string Issuer = "Issuer";

        public string GenerateToken(ApplicationUser user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email), "User email is required to generate a JWT token");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email)
            };

            var descriptorOptions = GetTokenDescriptorOptions();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(descriptorOptions.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(descriptorOptions.Expiration),
                Issuer = descriptorOptions.Issuer,
                Audience = descriptorOptions.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private static JwtTokenDescriptorsDto GetTokenDescriptorOptions()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            try
            {
                var jwtSection = configuration.GetSection(Jwt);
                return new JwtTokenDescriptorsDto
                {
                    Audience = jwtSection.GetValue<string>(Audience)!,
                    Issuer = jwtSection.GetValue<string>(Issuer)!,
                    Secret = jwtSection.GetValue<string>(SecretKey)!,
                    Expiration = jwtSection.GetValue<int>(ExpirationInMinutes)

                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
    }

