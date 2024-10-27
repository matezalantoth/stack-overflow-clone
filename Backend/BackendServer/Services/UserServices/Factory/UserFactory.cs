using BackendServer.Models.UserModels;
using BackendServer.Models.UserModels.DTOs;

using Microsoft.AspNetCore.Identity;

namespace BackendServer.Services.UserServices.Factory;

public class UserFactory : IUserFactory
{
    public User CreateUser(NewUser newUser)
    {
        return new User
        {
            Name = newUser.Name, UserName = newUser.UserName, Email = newUser.Email,
            DoB = newUser.DoB, Karma = 0
        };
    }
}