using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IUserRepository
{
    public void CreateUser(User user);

    public ValueTask<User?> GetUserById(Guid id);

    public ValueTask<User?> GetUserByEmail(string email);

    public Task<bool> AreCredentialsTaken(string email, string username);

    public Guid GetNewSessionToken();

    public Guid LoginUser(User user);

    public bool IsUserLoggedIn(Guid sessionToken);

    public void LogoutUser(Guid sessionToken);

    public ValueTask<User?> GetUserBySessionToken(Guid sessionToken);
    
    public void UpdateKarma(User user, int karma);
}