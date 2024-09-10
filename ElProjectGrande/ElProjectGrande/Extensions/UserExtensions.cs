using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {
        var answers = new List<AnswerDTO>();
        var questions = new List<QuestionDTO>();
        var karma = 0;
        var upvotes = new List<Guid>();
        var downvotes = new List<Guid>();

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

        if (user.Upvotes != null && user.Upvotes.Count != 0)
        {
            upvotes = user.Upvotes.Select(upvote => upvote).ToList();
        }

        if (user.Downvotes != null && user.Downvotes.Count != 0)
        {
            downvotes = user.Downvotes.Select(upvote => upvote).ToList();
        }

        return new UserDTO
        {
            UserName = user.UserName, Name = user.Name, SessionToken = user.SessionToken, Answers = answers,
            Email = user.Email, Questions = questions, Karma = karma, Upvotes = upvotes, Downvotes = downvotes
        };
    }
}