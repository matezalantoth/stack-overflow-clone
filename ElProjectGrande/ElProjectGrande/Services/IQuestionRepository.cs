using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions();

    public Question? GetQuestionById(Guid id);

    public QuestionDTO CreateQuestion(Question question);

    public void DeleteQuestion(Question question, User user);

    public QuestionDTO UpdateQuestion(Question question);
}