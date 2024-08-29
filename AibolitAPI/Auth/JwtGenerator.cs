using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AibolitAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace AibolitAPI.Auth;

public static class JwtGenerator
{
    public static string GenerateJwt(User user, string secretKey, DateTime expiryDate)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = CreateTokenDescriptor(claims, secretKey, expiryDate);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private static JwtSecurityToken CreateTokenDescriptor(IEnumerable<Claim> claims, string secretKey, DateTime expiryDate)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        return new JwtSecurityToken(
            claims: claims,
            expires: expiryDate,
            signingCredentials: credential
        );
    }
}