using ElProjectGrande.Extensions;
using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ElProjectGrande.Services.UserServices.Factory;

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