using ElProjectGrande.Data;
using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class QuestionRepository(ApiDbContext context) : IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return context.Questions.Select(q => new QuestionDTO
            { Id = q.Id, Content = q.Content, Username = q.User.Name });
    }
}