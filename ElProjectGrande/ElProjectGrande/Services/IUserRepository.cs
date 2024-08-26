using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IUserRepository
{
    public void CreateUser(User user);

    public ValueTask<User?> GetUserById(Guid id);
}