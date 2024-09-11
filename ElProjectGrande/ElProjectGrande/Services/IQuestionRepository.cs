using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions();

    public Task<Question?> GetQuestionById(Guid id);

    public QuestionDTO CreateQuestion(Question question, User user);

    public void DeleteQuestion(Question question, User user);

    public QuestionDTO UpdateQuestion(Question question);

    public IEnumerable<QuestionDTO> GetTrendingQuestions();
}