using ElProjectGrande.Extensions;
using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class UserFactory(IUserVerifier userVerifier) : IUserFactory
{
    public User CreateUser(NewUser newUser)
    {
        var password = userVerifier.HashPassword(newUser.Password, out var salt);
        var user = new User
        {
            Id = Guid.NewGuid(), Name = newUser.Name, UserName = newUser.UserName, Email = newUser.Email,
            Password = password,
            DoB = newUser.DoB, Salt = salt
        };
        return user;
    }

    public UserDTO CreateUserDTO(User user)
    {
        return user.ToDTO();
    }
}