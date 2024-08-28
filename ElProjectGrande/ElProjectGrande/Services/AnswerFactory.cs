using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class AnswerFactory : IAnswerFactory
{
    public Answer CreateAnswer(NewAnswer newAnswer, Question question, User user)
    {
        return new Answer
        {
            Id = Guid.NewGuid(), Content = newAnswer.Content, PostedAt = newAnswer.PostedAt, User = user,
            UserId = user.Id, Question = question, QuestionId = question.Id
        };
    }

    public Answer UpdateAnswer(string newContent, Answer answer)
    {
        answer.Content = newContent;
        return answer;
    }
}