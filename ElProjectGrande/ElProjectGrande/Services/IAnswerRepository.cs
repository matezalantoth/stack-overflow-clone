using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IAnswerRepository
{
    public Task<IEnumerable<AnswerDTO>> GetAllAnswersFromQuestion(Guid id);

    public Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question);
}