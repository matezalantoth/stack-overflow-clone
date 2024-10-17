using ElProjectGrande.Data;
using ElProjectGrande.Exceptions;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.QuestionServices.Repository;

public class QuestionRepository(ApiDbContext context) : IQuestionRepository
{
    public async Task<bool> CheckIfQuestionExists(Guid id)
    {
        return await context.Questions.FirstOrDefaultAsync(q => q.Id == id) != null;
    }

    public Task<Question?> GetQuestionById(Guid id)
    {
        return context.Questions
            .Include(q => q.User)
            .Include(q => q.Answers)
            .Include(q => q.Tags)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public QuestionDTO CreateQuestion(NewQuestion newQuestion, User user)

    {
        var tags = context.Tags.Where(x => newQuestion.Tags.Select(t => t.TagName).Contains(x.TagName)).ToList();
         var question = new Question
        {
            Title = newQuestion.Title, Content = newQuestion.Content, Id = Guid.NewGuid(), User = user,
            UserId = user.Id, PostedAt = newQuestion.PostedAt, Answers = [], Tags = tags
        };
        user.Questions.Add(question);
        context.Questions.Add(question);
        foreach (var tag in question.Tags) tag.Questions.Add(question);
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
        foreach (var tag in question.Tags) tag.Questions.Remove(question);
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
        var sevenDaysAgo = DateTime.Today.AddDays(-7);
        return context.Questions
            .Include(q => q.Tags)
            .Include(q => q.Answers)
            .Include(q => q.User)
            .OrderByDescending(q => q.Answers.Count(ans => ans.PostedAt >= sevenDaysAgo))
            .Take(5)
            .Select(q => q.ToDTO());
    }

    public IEnumerable<QuestionDTO> GetTenQuestion(int startIndex)
    {
        return context.Questions
            .Include(q => q.Answers)
            .Include(q => q.User)
            .Include(q => q.Tags)
            .Skip(startIndex).Take(startIndex + 10).Select(q => q.ToDTO());
    }

    public IEnumerable<QuestionDTO> GetQuestionsByTitle(string titleSubstring)
    {
        var closestTitles = Process.ExtractSorted(titleSubstring, context.Questions.Select(q => q.Title).ToArray())
            .Select(res => res.Value)
            .Take(10);

        var questions = context.Questions
            .Include(q => q.User)
            .Include(q => q.Answers)
            .Include(q => q.Tags);
        return closestTitles
            .Select(title =>
                questions.FirstOrDefault(q => q.Title == title))
            .Select(q => q?.ToDTO() ?? throw new NotFoundException("This question could not be found"));
    }

    public IEnumerable<QuestionDTO> GetQuestionsByContent(string contentSubstring)
    {
        var closestContents = Process
            .ExtractSorted(contentSubstring, context.Questions.Select(q => q.Content).ToArray())
            .Select(res => res.Value)
            .Take(10);
        var questions = context.Questions
            .Include(q => q.Tags)
            .Include(q => q.User)
            .Include(q => q.Answers);
        return closestContents
            .Select(content => questions.FirstOrDefault(q => q.Content == content))
            .Select(q => q?.ToDTO() ?? throw new NotFoundException("This question could not be found"));
    }
}