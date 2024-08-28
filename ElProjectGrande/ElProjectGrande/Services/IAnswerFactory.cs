using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IAnswerFactory
{
    public Answer CreateAnswer(NewAnswer newAnswer, Question question, User user);
}