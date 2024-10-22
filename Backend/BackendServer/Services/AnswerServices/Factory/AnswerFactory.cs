using BackendServer.Models.AnswerModels;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels;
using BackendServer.Models.UserModels;

namespace BackendServer.Services.AnswerServices.Factory;

public class AnswerFactory : IAnswerFactory
{
    public Answer CreateAnswer(NewAnswer newAnswer, Question question, User user)
    {
        return new Answer
        {
            Id = Guid.NewGuid(), Content = newAnswer.Content, PostedAt = newAnswer.PostedAt, User = user,
            UserId = user.Id, Question = question, QuestionId = question.Id, Votes = 0
        };
    }

    public Answer UpdateAnswer(string newContent, Answer answer)
    {
        answer.Content = newContent;
        return answer;
    }
}