using ElProjectGrande.Data;
using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class QuestionRepository(ApiDbContext context) : IQuestionRepository
{
    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return context.Questions.Select(q => new QuestionDTO
            { Content = q.Content, Username = q.User.UserName, Title = q.Title, PostedAt = q.PostedAt });
    }

    public QuestionDTO CreateQuestion(Question question)
    {
        var user = context.Users.FirstOrDefault(u => u.Id == question.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found!");
        }

        user.Questions.Add(question);
        context.Questions.Add(question);
        context.SaveChanges();
        return new QuestionDTO
        {
            Title = question.Title, Content = question.Content, Username = question.User.UserName,
            PostedAt = question.PostedAt
        };
    }
}