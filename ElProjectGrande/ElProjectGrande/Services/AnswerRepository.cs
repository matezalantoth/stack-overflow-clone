using ElProjectGrande.Extensions;
using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class AnswerRepository(IQuestionRepository questionRepository) : IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Guid id)
    {
        var question = questionRepository.GetQuestionById(id);
        if (question == null)
        {
            throw new ArgumentException($"Question of id {id} could not be found");
        }

        return question.Answers.Select(a => a.ToDTO());
    }
}