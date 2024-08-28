using ElProjectGrande.Data;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class AnswerRepository(IQuestionRepository questionRepository, ApiDbContext dbContext) : IAnswerRepository
{
    public async Task<IEnumerable<AnswerDTO>> GetAllAnswersFromQuestion(Guid id)
    {
        var question = await questionRepository.GetQuestionById(id);
        if (question == null)
        {
            throw new ArgumentException($"Question of id {id} could not be found");
        }

        return question.Answers.Select(a => a.ToDTO());
    }

    public async Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question)
    {
        dbContext.Answers.Add(answer);
        user.Answers.Add(answer);
        question.Answers.Add(answer);
        await dbContext.SaveChangesAsync();
        return answer.ToDTO();
    }
}