using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions();

    public QuestionDTO CreateQuestion(Question question);
}