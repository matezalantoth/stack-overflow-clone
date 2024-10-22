

using BackendServer.Models.UserModels;
using BackendServer.Models.UserModels.DTOs;

namespace BackendServer.Services.UserServices.Factory;

public interface IUserFactory
{
    public User CreateUser(NewUser user);
}