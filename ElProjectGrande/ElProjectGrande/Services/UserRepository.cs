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
}