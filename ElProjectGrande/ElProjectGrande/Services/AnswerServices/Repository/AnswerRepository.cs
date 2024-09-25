using ElProjectGrande.Data;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.UserModels;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.AnswerServices.Repository;

public class AnswerRepository(ApiDbContext dbContext) : IAnswerRepository
{
    public async Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question)
    {
        dbContext.Answers.Add(answer);
        user.Answers.Add(answer);
        question.Answers.Add(answer);
        await dbContext.SaveChangesAsync();
        return answer.ToDTO();
    }

    public IEnumerable<AnswersOfQuestionDTO> GetAllAnswersByQuestionId(Guid questionId)
    {
        return dbContext.Answers.Where(a => a.QuestionId == questionId)
            .Include(a => a.User).Select(a => a.ToAnswerOfQuestionDTO());
    }

    public Task<Answer?> GetAnswerById(Guid id)
    {
        return dbContext.Answers
            .Include(a => a.User)
            .Include(a => a.Question)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task DeleteAnswer(Answer answer, User user)
    {
        dbContext.Remove(answer);
        user.Answers.Remove(answer);
        answer.Question.Answers.Remove(answer);
        await dbContext.SaveChangesAsync();
    }

    public async Task<AnswersOfQuestionDTO> UpdateAnswer(Answer answer)
    {
        dbContext.Update(answer);
        await dbContext.SaveChangesAsync();
        return answer.ToAnswerOfQuestionDTO();
    }

    public async Task<AnswersOfQuestionDTO> AcceptAnswer(Answer answer)
    {
        answer.Accepted = true;
        await dbContext.SaveChangesAsync();
        return answer.ToAnswerOfQuestionDTO();
    }

    public void VoteAnswer(Answer answer, int vote)
    {
        answer.Votes += vote;
        dbContext.Update(answer);
    }

    public IEnumerable<AnswerDTO> GetAnswersByContent(string contentSubstring)
    {
        var bestResults = Process.ExtractSorted(contentSubstring, dbContext.Answers.Select(a => a.Content).ToArray())
            .Take(10)
            .Select(res => res.Value);

        return dbContext.Answers.Where(a => bestResults.Contains(a.Content)).Select(a => a.ToDTO());
    }

    public IEnumerable<AnswerDTO> GetAllAnswersFromQuestion(Question question)
    {
        return question.Answers.Select(a => a.ToDTO());
    }
}