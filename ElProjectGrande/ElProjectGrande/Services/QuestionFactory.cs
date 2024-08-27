using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class QuestionFactory : IQuestionFactory
{
    public Question CreateQuestion(NewQuestion newQuestion, User user)
    {
        return new Question
        {
            Title = newQuestion.Title, Content = newQuestion.Content, Id = Guid.NewGuid(), User = user,
            UserId = user.Id, PostedAt = newQuestion.PostedAt, Answers = []
        };
    }
}