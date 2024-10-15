using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ElProjectGrande.Services.AuthenticationServices.TokenService;

public class TokenService : ITokenService
{
    private const int ExpirationMinutes = 1440;

    public string CreateToken(IdentityUser user, string role)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(user, role),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public string ValidateAndGetSessionToken(string sessionToken)
    {
        if (sessionToken == string.Empty || !sessionToken.StartsWith("Bearer "))
            throw new UnauthorizedAccessException("Invalid session token");

        return sessionToken["Bearer ".Length..].Trim();
    }

    private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
        DateTime expiration)
    {
        return new JwtSecurityToken(
            "Grande",
            "Grande",
            claims,
            expires: expiration,
            signingCredentials: credentials
        );
    }

    private List<Claim> CreateClaims(IdentityUser user, string? role)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, "SessionToken"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat,
                    EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture),
                    ClaimValueTypes.Integer64),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };
            if (role != null) claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var issuingKey = "";
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
        {
            issuingKey = "xR3!m9QpT4hK8jLa!Vw6D%Zc5N2fUrT3D";
        }
        else
        {
            issuingKey = Environment.GetEnvironmentVariable("ISSUING_KEY") ??
                         throw new Exception("ISSUING_KEY not found");
        }
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(issuingKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}