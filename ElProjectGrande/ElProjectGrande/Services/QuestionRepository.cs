using ElProjectGrande.Data;
using ElProjectGrande.Models;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services;

public class QuestionRepository(ApiDbContext context) : IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return context.Questions.Select(q => new QuestionDTO
            { Content = q.Content, Username = q.User.UserName, Title = q.Title, PostedAt = q.PostedAt, Id = q.Id });
    }

    public Task<Question?> GetQuestionById(Guid id)
    {
        return context.Questions
            .Include(q => q.User)
            .Include(q => q.Answers)
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
}