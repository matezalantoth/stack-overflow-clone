using ElProjectGrande.Data;
using ElProjectGrande.Exceptions;
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

    public IEnumerable<AnswerDTO> GetAllAnswersByQuestionId(Guid questionId)
    {
        return dbContext.Answers.Where(a => a.QuestionId == questionId)
            .Include(a => a.User)
            .Select(a => a.ToDTO());
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

    public IEnumerable<AdminAnswerDTO> GetAnswersByContent(string contentSubstring)
    {
        var bestResults = Process.ExtractSorted(contentSubstring, dbContext.Answers.Select(a => a.Content).ToArray())
            .Select(res => res.Value)
            .Take(10);

        var answers =
            dbContext.Answers
                .Include(a => a.User);
        return bestResults
            .Select(content =>
                answers
                    .FirstOrDefault(a =>
                        a.Content == content.ToString()))
            .Select(a =>
            a?.ToAdminDTO() ?? throw new NotFoundException("This answer could not be found"));
    }

    public async Task<Answer> UnAcceptAnswer(Guid answerId)
    {
        var answer =
            await dbContext.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId) ?? throw new NotFoundException("This answer could not be found");
        answer.Accepted = false;
        await dbContext.SaveChangesAsync();
        return answer;
    }

    public async Task<Question> GetQuestionOfAnswerByAnswerId(Guid answerId)
    {
        return (
            await dbContext.Answers
                .Include(answer => answer.Question)
                .FirstOrDefaultAsync(a => a.Id == answerId) ??
                throw new NotFoundException("This answer could not be found")).Question;
    }
}