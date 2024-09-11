using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class AnswerExtensions
{
    public static AnswerDTO ToDTO(this Answer answer)
    {
        Console.WriteLine($"Answer Id: {answer.Id}");
        Console.WriteLine($"Answer Content: {answer.Content}");
        Console.WriteLine($"Answer PostedAt: {answer.PostedAt}");
        Console.WriteLine($"Answer Username: {answer.User.UserName}");
        Console.WriteLine($"Answer Question: {answer.Question.ToDTO()}");
        Console.WriteLine($"Answer Accepted: {answer.Accepted}");
        return new AnswerDTO
        {
            Id = answer.Id,
            Content = answer.Content, PostedAt = answer.PostedAt, Username = answer.User.UserName,
            Question = answer.Question.ToDTO(), Accepted = answer.Accepted
        };
    }
}