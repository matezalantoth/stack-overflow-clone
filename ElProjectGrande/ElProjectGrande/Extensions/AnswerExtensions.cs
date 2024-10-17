using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.AnswerModels.DTOs;

namespace ElProjectGrande.Extensions;

public static class AnswerExtensions
{
    public static AnswerDTO ToDTO(this Answer answer)
    {
        return new AnswerDTO
        {
            Id = answer.Id,
            Content = answer.Content, PostedAt = answer.PostedAt, Username = answer.User.UserName,
            Accepted = answer.Accepted, Votes = answer.Votes
        };
    }

    public static AdminAnswerDTO ToAdminDTO(this Answer answer)
    {
        return new AdminAnswerDTO
        {
            Id = answer.Id,
            Content = answer.Content,
            Question = answer.Question.ToUpdatedDTO()
        };
    }
}