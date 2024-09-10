using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class AnswerExtensions
{
    public static AnswerDTO ToDTO(this Answer answer)
    {
        return new AnswerDTO
        {
            Id = answer.Id,
            Content = answer.Content, PostedAt = answer.PostedAt, Username = answer.User.UserName,
            Question = answer.Question.ToDTO(), Accepted = answer.Accepted, Votes = answer.Votes
        };
    }
}