using ElProjectGrande.Data;
using ElProjectGrande.Models;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services;

public class UserRepository(ApiDbContext context) : IUserRepository
{
    public void CreateUser(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }


    public async ValueTask<User?> GetUserById(Guid id)
    {
        return await context.Users
            .Include(u => u.Questions)
            .Include(u => u.Answers)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Guid GetNewSessionToken()
    {
        return Guid.NewGuid();
    }

    public bool IsUserLoggedIn(Guid sessionToken)
    {
        return context.Users.FirstOrDefault(u => u.SessionToken == sessionToken) != null;
    }

    public void LogoutUser(Guid sessionToken)
    {
        var user = context.Users.FirstOrDefault(u => u.SessionToken == sessionToken);
        if (user == null)
        {
            throw new ArgumentException("This session token could not be found");
        }

        user.SessionToken = Guid.Empty;
        context.Users.Update(user);
        context.SaveChanges();
    }

    public ValueTask<User?> GetUserBySessionToken(Guid sessionToken)
    {
        var partialUser = context.Users.FirstOrDefault(u => u.SessionToken == sessionToken);
        if (partialUser == null)
        {
            throw new ArgumentException("This session token is invalid");
        }

        return GetUserById(partialUser.Id);
    }
}