using ElProjectGrande.Models;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.QuestionServices.Repository;

public interface IQuestionRepository
{
    public Task<bool> CheckIfQuestionExists(Guid id);
    public IEnumerable<QuestionDTO> GetQuestions();

    public Task<Question?> GetQuestionById(Guid id);

    public Task<Question?> GetQuestionByIdWithoutAnswers(Guid id);

    public QuestionDTO CreateQuestion(Question question, User user);

    public void DeleteQuestion(Question question, User user);

    public QuestionDTO UpdateQuestion(Question question);

    public IEnumerable<QuestionDTO> GetTrendingQuestions();

    public IEnumerable<QuestionDTO> GetTenQuestion(int startIndex);
}