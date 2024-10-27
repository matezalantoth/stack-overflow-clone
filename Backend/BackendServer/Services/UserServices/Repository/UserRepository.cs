
using BackendServer.Data;
using BackendServer.Exceptions;
using BackendServer.Models.UserModels;
using BackendServer.Services.AuthenticationServices.TokenService;
using FuzzySharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Services.UserServices.Repository;

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
        if (managedUser == null) throw new BadRequestException("Invalid credentials");
        var isPasswordValid = await userManager.CheckPasswordAsync(managedUser, password);
        if (!isPasswordValid) throw new BadRequestException("Invalid credentials");
        var roles = await userManager.GetRolesAsync(managedUser);
        var token = tokenService.CreateToken(managedUser, roles[0]);
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
            .ThenInclude(q => q.Tags)
            .Include(u => u.Answers)
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
        return await context.Users
            .Include(u => u.Questions)
            .ThenInclude(q => q.Tags)
            .Include(u => u.Answers)
            .FirstOrDefaultAsync(u => u.UserName == username);
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
            .ThenInclude(q => q.Tags)
            .FirstOrDefaultAsync(u => u.SessionToken == sessionToken);
    }

    public async ValueTask<User> BanUserByUsername(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
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
            if (user.MutedUntil > now) throw new ForbiddenException("You are muted, please contact support");

            await UnMuteUserByUsername(user.UserName ?? throw new BadRequestException("Something went wrong!"));
        }

        if (user.Banned) throw new ForbiddenException("You are banned, please contact support");
    }

    public async ValueTask<User> MuteUserByUsername(string username, int mutedFor)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null) throw new NotFoundException("This user could not be found");
        user.Muted = true;
        user.MutedUntil = user.MutedUntil == null ? DateTime.Now.AddHours(mutedFor) : user.MutedUntil?.AddHours(mutedFor);
        await userManager.UpdateAsync(user);
        return user;
    }

    public async ValueTask<User> UnBanUserByUsername(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null) throw new NotFoundException("This user could not be found");
        user.Banned = false;
        await userManager.UpdateAsync(user);
        return user;
    }

    public async ValueTask<User> UnMuteUserByUsername(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == username) ??
                   throw new NotFoundException("User could not be found");
        user.Muted = false;
        await userManager.UpdateAsync(user);
        return user;
    }

    public IEnumerable<string> GetUsersWithSimilarUsernames(string usernameSubstring)
    {
        var bestResults = Process.ExtractSorted(usernameSubstring, context.Users.Select(u => u.UserName))
            .Select(res => res.Value)
            .ToList()
            .Take(10);

        return bestResults;
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

    public async Task UpdateUser(User user, string? password)
    {
        if (password != null)
        {
            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, password);
        }
        var result = await userManager.UpdateAsync(user);
      
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user");
        }
    }

    public async Task<bool>  VerifyUser(User user, string password)
    {
        return  await userManager.CheckPasswordAsync(user, password);
        
         
    }
}