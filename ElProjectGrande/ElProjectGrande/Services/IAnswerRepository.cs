using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Guid id);
}