using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodePulse.API.Models {
    public interface ITokenRepository {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }

    public class TokenRepository : ITokenRepository {
        private IConfiguration configuration;
        public TokenRepository(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public string CreateJWTToken(IdentityUser user, List<string> roles) {
            // create claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            // define jwt security parameters
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
