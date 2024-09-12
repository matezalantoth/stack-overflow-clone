using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IUserFactory
{
    public User CreateUser(NewUser user);

    public UserDTO CreateUserDTO(User user);
}