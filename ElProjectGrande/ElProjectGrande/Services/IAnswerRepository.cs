using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Question question);

    public Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question);

    public Task<Answer?> GetAnswerById(Guid id);

    public void DeleteAnswer(Answer answer, User user);

    public Task<AnswerDTO> UpdateAnswer(Answer answer);
}