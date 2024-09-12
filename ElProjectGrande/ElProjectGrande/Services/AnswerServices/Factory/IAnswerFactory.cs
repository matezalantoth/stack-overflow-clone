using ElProjectGrande.Models;
using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.AnswerServices.Factory;

public interface IAnswerFactory
{
    public Answer CreateAnswer(NewAnswer newAnswer, Question question, User user);

    public Answer UpdateAnswer(string newContent, Answer answer);
}