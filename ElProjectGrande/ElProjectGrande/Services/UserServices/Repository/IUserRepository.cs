using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;

namespace ElProjectGrande.Services.UserServices.Repository;

public interface IUserRepository
{
    public Task CreateUser(User user, string password, string role);

    public ValueTask<User> UnMuteUserById(string userId);

    public Task<bool> AreCredentialsTaken(string email, string username);

    public Task<string> LoginUser(string email, string password);

    public bool IsUserLoggedIn(string sessionToken);

    public Task LogoutUser(string sessionToken);

    public ValueTask<User?> GetUserBySessionToken(string sessionToken);

    public Task CheckIfUserIsMutedOrBanned(User user);

    public Task UpdateKarma(User user, int karma);

    public Task Upvote(User user, Guid answerGuid);

    public Task Downvote(User user, Guid answerGuid);

    public Task RemoveUpvote(User user, Guid answerGuid);

    public Task RemoveDownvote(User user, Guid answerGuid);

    public ValueTask<User?> GetUserByUserName(string username);

    public ValueTask<User?> GetUserBySessionTokenOnlyAnswers(string sessionToken);

    public ValueTask<User?> GetUserBySessionTokenOnlyQuestions(string sessionToken);

    public ValueTask<User> BanUserById(string userId);

    public ValueTask<User> MuteUserById(string userId, int mutedFor);

    public ValueTask<User> UnBanUserById(string userId);

    public IEnumerable<UserDTO> GetUsersWithSimilarUsernames(string usernameSubstring);
}