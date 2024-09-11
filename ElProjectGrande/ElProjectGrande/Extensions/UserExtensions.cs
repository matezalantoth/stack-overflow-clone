using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {


        return new UserDTO
        {
            UserName = user.UserName, Name = user.Name, SessionToken = user.SessionToken,
            Answers = user.Answers.Select(answer => answer.ToDTO()).ToList(),
            Email = user.Email, Questions = user.Questions.Select(question => question.ToDTO()).ToList(),
            Karma = user.Karma
        };
    }
}