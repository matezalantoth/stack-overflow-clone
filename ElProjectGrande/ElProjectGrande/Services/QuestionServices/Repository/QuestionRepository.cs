using ElProjectGrande.Data;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.QuestionServices.Repository;

public class QuestionRepository(ApiDbContext context) : IQuestionRepository
{
    public async Task<bool> CheckIfQuestionExists(Guid id)
    {
        return await context.Questions.FirstOrDefaultAsync(q => q.Id == id) != null;
    }

    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return context.Questions
            .Include(q => q.User)
            .Select(q => q.ToDTO());
    }

    public Task<Question?> GetQuestionById(Guid id)
    {
        return context.Questions
            .Include(q => q.User)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public Task<Question?> GetQuestionByIdWithoutAnswers(Guid id)
    {
        return context.Questions
            .Include(q => q.User)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public QuestionDTO CreateQuestion(Question question, User user)
    {
        user.Questions.Add(question);
        context.Questions.Add(question);
        context.SaveChanges();
        return new QuestionDTO
        {
            Title = question.Title, Content = question.Content, Username = question.User.UserName,
            PostedAt = question.PostedAt, Id = question.Id
        };
    }

    public void DeleteQuestion(Question question, User user)
    {
        user.Questions.Remove(question);
        context.Questions.Remove(question);
        context.SaveChanges();
    }

    public QuestionDTO UpdateQuestion(Question question)
    {
        context.Update(question);
        context.SaveChanges();
        return new QuestionDTO
        {
            Id = question.Id, Title = question.Title, Content = question.Content, PostedAt = question.PostedAt,
            Username = question.User.UserName
        };
    }

    public IEnumerable<QuestionDTO> GetTrendingQuestions()
    {
        DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);
        return context.Questions
            .Include(q => q.Answers)
            .Include(q => q.User)
            .OrderByDescending(q => q.Answers.Count(ans => ans.PostedAt >= sevenDaysAgo))
            .Take(5)
            .Select(q => q.ToDTO());
    }
}