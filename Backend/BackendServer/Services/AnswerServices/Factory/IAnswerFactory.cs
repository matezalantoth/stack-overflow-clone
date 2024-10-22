using BackendServer.Models.AnswerModels;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels;
using BackendServer.Models.UserModels;

namespace BackendServer.Services.AnswerServices.Factory;

public interface IAnswerFactory
{
    public Answer CreateAnswer(NewAnswer newAnswer, Question question, User user);

    public Answer UpdateAnswer(string newContent, Answer answer);
}