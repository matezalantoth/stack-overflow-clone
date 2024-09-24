using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.UserServices.Repository;

public interface IUserRepository
{
    public Task CreateUser(User user, string password, string role);

    public Task<bool> AreCredentialsTaken(string email, string username);

    public Task<string> LoginUser(string email, string password);

    public bool IsUserLoggedIn(string sessionToken);

    public Task LogoutUser(string sessionToken);

    public ValueTask<User?> GetUserBySessionToken(string sessionToken);

    public Task UpdateKarma(User user, int karma);

    public Task Upvote(User user, Guid answerGuid);

    public Task Downvote(User user, Guid answerGuid);

    public Task RemoveUpvote(User user, Guid answerGuid);

    public Task RemoveDownvote(User user, Guid answerGuid);

    public ValueTask<User?> GetUserByUserName(string username);

    public ValueTask<User?> GetUserBySessionTokenOnlyAnswers(string sessionToken);

    public ValueTask<User?> GetUserBySessionTokenOnlyQuestions(string sessionToken);
}