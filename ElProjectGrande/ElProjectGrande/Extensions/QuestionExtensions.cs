using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class QuestionExtensions
{
    public static QuestionDTO ToDTO(this Question question)
    {
        Console.WriteLine($"Question Title: {question.Title}");
        Console.WriteLine($"Question Username: {question.User.UserName}");
        Console.WriteLine($"Question PostedAt: {question.PostedAt}");
        Console.WriteLine($"Question Id: {question.Id}");
        Console.WriteLine($"Question Content: {question.Content}");
        Console.WriteLine($"Question HasAccepted: {question.HasAccepted()}");
        return new QuestionDTO
        {
            Title = question.Title, Username = question.User.UserName, PostedAt = question.PostedAt, Id = question.Id,
            Content = question.Content, HasAccepted = question.HasAccepted()
        };
    }
}