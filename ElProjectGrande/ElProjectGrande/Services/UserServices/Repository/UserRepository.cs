using ElProjectGrande.Data;
using ElProjectGrande.Exceptions;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.AuthenticationServices.TokenService;
using FuzzySharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.UserServices.Repository;

public class UserRepository(UserManager<User> userManager, ApiDbContext context, ITokenService tokenService)
    : IUserRepository
{
    public async Task CreateUser(User user, string password, string role)
    {
        var result = await userManager.CreateAsync(user, password);
        if (result != IdentityResult.Success) throw new Exception(result.ToString());

        await userManager.AddToRoleAsync(user, "User");
    }

    public Task<bool> AreCredentialsTaken(string email, string username)
    {
        return context.Users.AnyAsync(u => u.Email == email || u.UserName == username);
    }

    public async Task<string> LoginUser(string email, string password)
    {
        var managedUser = await userManager.FindByEmailAsync(email);
        if (managedUser == null) throw new Exception("Invalid credentials");
        var isPasswordValid = await userManager.CheckPasswordAsync(managedUser, password);
        if (!isPasswordValid) throw new Exception("Invalid credentials");
        var roles = await userManager.GetRolesAsync(managedUser);
        var token = tokenService.CreateToken(managedUser, roles[0] ?? "User");
        managedUser.SessionToken = token;
        await userManager.UpdateAsync(managedUser);
        return token;
    }

    public bool IsUserLoggedIn(string sessionToken)
    {
        return context.Users.FirstOrDefault(u => u.SessionToken == sessionToken) != null;
    }

    public async Task LogoutUser(string sessionToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
        var managedUser = await userManager.FindByEmailAsync(user?.Email);
        if (user == null || managedUser == null) throw new ArgumentException("This session token could not be found");

        managedUser.SessionToken = string.Empty;
        await userManager.UpdateAsync(managedUser);
    }

    public async ValueTask<User?> GetUserBySessionToken(string sessionToken)
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

    public async Task UpdateKarma(User user, int karma)
    {
        user.Karma += karma;
        await userManager.UpdateAsync(user);
    }

    public async Task Upvote(User user, Guid answerGuid)
    {
        user.Upvotes.Add(answerGuid);
        await userManager.UpdateAsync(user);
    }

    public async Task Downvote(User user, Guid answerGuid)
    {
        user.Downvotes.Add(answerGuid);
        await userManager.UpdateAsync(user);
    }

    public async Task RemoveUpvote(User user, Guid answerGuid)
    {
        user.Upvotes.Remove(answerGuid);
        await userManager.UpdateAsync(user);
    }

    public async Task RemoveDownvote(User user, Guid answerGuid)
    {
        user.Downvotes.Remove(answerGuid);
        await userManager.UpdateAsync(user);
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

    public async ValueTask<User?> GetUserBySessionTokenOnlyAnswers(string sessionToken)
    {
        return await context.Users
            .Include(u => u.Answers)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public async ValueTask<User?> GetUserBySessionTokenOnlyQuestions(string sessionToken)
    {
        return await context.Users
            .Include(u => u.Questions)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public async ValueTask<User> BanUserById(string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("This user could not be found");
        user.Banned = true;
        await userManager.UpdateAsync(user);
        return user;
    }

    public async Task CheckIfUserIsMutedOrBanned(User user)
    {
        if (user.Muted)
        {
            var now = DateTime.Now;
            var unMutedAt = now.AddMinutes(user.MutedFor);
            if (unMutedAt < now) throw new UnauthorizedAccessException();

            await UnMuteUserById(user.Id);
        }

        if (user.Banned) throw new UnauthorizedAccessException();
    }

    public async ValueTask<User> MuteUserById(string userId, int mutedFor)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("This user could not be found");
        user.MutedFor += mutedFor;
        user.Muted = true;
        await userManager.UpdateAsync(user);
        return user;
    }

    public async ValueTask<User> UnBanUserById(string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new NotFoundException("This user could not be found");
        user.Banned = false;
        await userManager.UpdateAsync(user);
        return user;
    }

    public async ValueTask<User> UnMuteUserById(string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId) ??
                   throw new NotFoundException("User could not be found");
        user.Muted = false;
        await userManager.UpdateAsync(user);
        return user;
    }

    public IEnumerable<UserDTO> GetUsersWithSimilarUsernames(string usernameSubstring)
    {
        var bestResults = Process.ExtractSorted(usernameSubstring, context.Users.Select(u => u.UserName).ToArray())
            .ToList()
            .Select(res => res.Value)
            .Take(10);
        var users = context.Users.Include(u => u.Questions)
            .ThenInclude(q => q.Answers)
            .ThenInclude(a => a.User)
            .Include(u => u.Answers)
            .ThenInclude(a => a.Question)
            .ThenInclude(q => q.User).ToList();
        return bestResults
            .Select(username => users.FirstOrDefault(u => u.UserName == username))
            .Select(user => user?.ToDTO() ?? throw new NotFoundException("This user could not be found"));
    }

    public bool IsUserAdmin(User user)
    {
        return userManager.IsInRoleAsync(user, "Admin").GetAwaiter().GetResult();
    }

    public bool IsUserAdmin(string userId)
    {
        return userManager.IsInRoleAsync(
            context.Users.FirstOrDefault(u => u.Id == userId) ??
            throw new NotFoundException("This user could not be found"), "Admin").GetAwaiter().GetResult();
    }
}