using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;

namespace ElProjectGrande.Extensions;

public static class QuestionExtensions
{
    public static QuestionDTO ToDTO(this Question question)
    {
        return new QuestionDTO
        {
            Title = question.Title, Username = question.User.UserName, PostedAt = question.PostedAt, Id = question.Id,
            Content = question.Content, HasAccepted = question.HasAccepted(),
            Tags = question.Tags.Select(t => t.ToDTO()).ToList()
        };
    }

    public static UpdatedQuestion ToUpdatedDTO(this Question question)
    {
        return new UpdatedQuestion
        {
            Title = question.Title,
            Content = question.Content
        };
    }
}