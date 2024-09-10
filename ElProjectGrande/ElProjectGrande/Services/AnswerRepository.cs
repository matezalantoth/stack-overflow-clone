using ElProjectGrande.Data;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services;

public class AnswerRepository(ApiDbContext dbContext) : IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Question question)
    {
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

    public Task<Answer?> GetAnswerById(Guid id)
    {
        return dbContext.Answers
            .Include(a => a.User)
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async void DeleteAnswer(Answer answer, User user)
    {
        dbContext.Remove(answer);
        user.Answers.Remove(answer);
        answer.Question.Answers.Remove(answer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<AnswerDTO> UpdateAnswer(Answer answer)
    {
        dbContext.Update(answer);
        await dbContext.SaveChangesAsync();
        return answer.ToDTO();
    }

    public async Task<AnswerDTO> AcceptAnswer(Answer answer)
    {
        answer.Accepted = true;
        await dbContext.SaveChangesAsync();
        return answer.ToDTO();
    }

    public void VoteAnswer(Answer answer, int vote)
    {
        answer.Votes += vote;
        dbContext.Update(answer);
    }
}