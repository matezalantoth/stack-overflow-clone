using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {
        var answers = new List<AnswerDTO>();
        var questions = new List<QuestionDTO>();
        var karma = 0;

        if (user.Answers != null && user.Answers.Count != 0)
        {
            answers = user.Answers.Select(answer => answer.ToDTO()).ToList();
        }

        if (user.Questions != null && user.Questions.Count != 0)
        {
            questions = user.Questions.Select(question => question.ToDTO()).ToList();
        }

        if (user.Karma != null && user.Karma != 0)
        {
            karma = user.Karma;
        }

        return new UserDTO
        {
            UserName = user.UserName, Name = user.Name, SessionToken = user.SessionToken, Answers = answers,
            Email = user.Email, Questions = questions, Karma = karma
        };
    }
}