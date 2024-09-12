using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;

namespace ElProjectGrande.Services.UserServices.Factory;

public interface IUserFactory
{
    public User CreateUser(NewUser user);
}