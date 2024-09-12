using ElProjectGrande.Extensions;
using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.UserServices.Verifier;

namespace ElProjectGrande.Services.UserServices.Factory;

public class UserFactory(IUserVerifier userVerifier) : IUserFactory
{
    public User CreateUser(NewUser newUser)
    {
        var password = userVerifier.HashPassword(newUser.Password, out var salt);
        var user = new User
        {
            Id = Guid.NewGuid(), Name = newUser.Name, UserName = newUser.UserName, Email = newUser.Email,
            Password = password,
            DoB = newUser.DoB, Salt = salt, Karma = 0
        };
        return user;
    }
}