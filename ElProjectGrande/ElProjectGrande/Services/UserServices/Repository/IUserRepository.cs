using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.UserServices.Repository;

public interface IUserRepository
{
    public Task CreateUser(User user, string password);

    public ValueTask<User?> GetUserByEmail(string email);

    public Task<bool> AreCredentialsTaken(string email, string username);

    public Task<string> LoginUser(string email, string password);

    public bool IsUserLoggedIn(string sessionToken);

    public void LogoutUser(string sessionToken);

    public ValueTask<User?> GetUserBySessionToken(string sessionToken);

    public void UpdateKarma(User user, int karma);

    public void Upvote(User user, Guid answerGuid);

    public void Downvote(User user, Guid answerGuid);

    public void RemoveUpvote(User user, Guid answerGuid);

    public void RemoveDownvote(User user, Guid answerGuid);

    public ValueTask<User?> GetUserByUserName(string username);

    public ValueTask<User?> GetUserBySessionTokenOnlyAnswers(string sessionToken);

    public ValueTask<User?> GetUserBySessionTokenOnlyQuestions(string sessionToken);
}