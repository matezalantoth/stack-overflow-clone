using BackendServer.Models.UserModels;

namespace BackendServer.Services.AuthenticationServices.TokenService;

public interface ITokenService
{
    public string CreateToken(User user, string role);

    public string ValidateAndGetSessionToken(string sessionToken);
}