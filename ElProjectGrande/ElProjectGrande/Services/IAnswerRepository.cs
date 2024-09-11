using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Question question);

    public IEnumerable<AnswerDTO> GetAllAnswersByQuestionId(Guid questionId);

    public Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question);

    public Task<Answer?> GetAnswerById(Guid id);

    public void DeleteAnswer(Answer answer, User user);

    public Task<AnswerDTO> UpdateAnswer(Answer answer);

    public Task<AnswerDTO> AcceptAnswer(Answer answer);
}