using ElProjectGrande.Data;
using ElProjectGrande.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.UserServices.Repository;

public class UserRepository(ApiDbContext context) : IUserRepository
{
    public void CreateUser(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public async ValueTask<User?> GetUserByEmail(string email)
    {
        return await context.Users
            .Include(u => u.Questions)
            .ThenInclude(q => q.Answers)
            .ThenInclude(a => a.User)
            .Include(u => u.Answers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.User)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task<bool> AreCredentialsTaken(string email, string username)
    {
        return context.Users.AnyAsync(u => u.Email == email || u.UserName == username);
    }

    public Guid GetNewSessionToken()
    {
        return Guid.NewGuid();
    }

    public Guid LoginUser(User user)
    {
        user.SessionToken = GetNewSessionToken();
        context.SaveChanges();
        return user.SessionToken;
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

    public async ValueTask<User?> GetUserBySessionToken(Guid sessionToken)
    {
        return await context.Users
            .Include(u => u.Questions)
            .ThenInclude(q => q.Answers)
            .ThenInclude(a => a.User)
            .Include(u => u.Answers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.User)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public void UpdateKarma(User user, int karma)
    {
        user.Karma += karma;
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void Upvote(User user, Guid answerGuid)
    {
        user.Upvotes.Add(answerGuid);
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void Downvote(User user, Guid answerGuid)
    {
        user.Downvotes.Add(answerGuid);
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void RemoveUpvote(User user, Guid answerGuid)
    {
        user.Upvotes.Remove(answerGuid);
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void RemoveDownvote(User user, Guid answerGuid)
    {
        user.Downvotes.Remove(answerGuid);
        context.Users.Update(user);
        context.SaveChanges();
    }

    public async ValueTask<User?> GetUserByUserName(string username)

    {
        return await context.Users.Include(u => u.Questions)
            .ThenInclude(q => q.Answers)
            .ThenInclude(a => a.User)
            .Include(u => u.Answers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.User).FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async ValueTask<User?> GetUserBySessionTokenOnlyAnswers(Guid sessionToken)
    {
        return await context.Users
            .Include(u => u.Answers)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public async ValueTask<User?> GetUserBySessionTokenOnlyQuestions(Guid sessionToken)
    {
        return await context.Users
            .Include(u => u.Questions)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public async ValueTask<Guid?> GetUserIdBySessionToken(Guid sessionToken)
    {
        return (await context.Users
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken))?.Id;
    }
}