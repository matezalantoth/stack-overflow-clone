using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.UserServices.Repository;

public interface IUserRepository
{
    public void CreateUser(User user);

    public ValueTask<User?> GetUserByEmail(string email);

    public Task<bool> AreCredentialsTaken(string email, string username);

    public Guid LoginUser(User user);

    public bool IsUserLoggedIn(Guid sessionToken);

    public void LogoutUser(Guid sessionToken);

    public ValueTask<User?> GetUserBySessionToken(Guid sessionToken);

    public void UpdateKarma(User user, int karma);

    public void Upvote(User user, Guid answerGuid);

    public void Downvote(User user, Guid answerGuid);

    public void RemoveUpvote(User user, Guid answerGuid);

    public void RemoveDownvote(User user, Guid answerGuid);

    public ValueTask<User?> GetUserByUserName(string username);

    public ValueTask<User?> GetUserBySessionTokenOnlyAnswers(Guid sessionToken);

    public ValueTask<User?> GetUserBySessionTokenOnlyQuestions(Guid sessionToken);

    public ValueTask<Guid?> GetUserIdBySessionToken(Guid sessionToken);
}