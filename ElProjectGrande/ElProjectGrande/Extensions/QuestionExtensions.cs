using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class QuestionExtensions
{
    public static QuestionDTO ToDTO(this Question question)
    {
        return new QuestionDTO
        {
            Title = question.Title, Username = question.User.UserName, PostedAt = question.PostedAt, Id = question.Id,
            Content = question.Content
        };
    }
}