

using BackendServer.Models.QuestionModels;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Models.UserModels;

namespace BackendServer.Services.QuestionServices.Repository;

public interface IQuestionRepository
{
    public Task<bool> CheckIfQuestionExists(Guid id);

    public Task<Question?> GetQuestionById(Guid id);

    public QuestionDTO CreateQuestion(NewQuestion newQuestion, User user);

    public void DeleteQuestion(Question question, User user);

    public QuestionDTO UpdateQuestion(Question question);

    public IEnumerable<QuestionDTO> GetTrendingQuestions();

    public IEnumerable<QuestionDTO> GetTenQuestion(int startIndex);

    public IEnumerable<QuestionDTO> GetQuestionsByTitle(string titleSubstring);

    public IEnumerable<QuestionDTO> GetQuestionsByContent(string contentSubstring);
}