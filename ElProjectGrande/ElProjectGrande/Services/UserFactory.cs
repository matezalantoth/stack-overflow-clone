using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class UserFactory(IUserVerifier userVerifier) : IUserFactory
{
    public User CreateUser(NewUser newUser)
    {
        var password = userVerifier.HashPassword(newUser.Password, out var salt);
        var user = new User
        {
            Id = Guid.NewGuid(), Name = newUser.Name, UserName = newUser.UserName, Email = newUser.Email,
            Password = password,
            DoB = newUser.DoB, Salt = salt
        };
        return user;
    }

    public UserDTO CreateUserDTO(User user)
    {
        var answers = new List<AnswerDTO>();
        var questions = new List<QuestionDTO>();

        if (user.Answers != null && user.Answers.Count != 0)
        {
            answers = user.Answers.Select(answer => new AnswerDTO
            {
                Content = answer.Content,
                Username = user.UserName,
                PostedAt = answer.PostedAt
            }).ToList();
        }

        if (user.Questions != null && user.Questions.Count != 0)
        {
            questions = user.Questions.Select(question => new QuestionDTO
            {
                Content = question.Content,
                PostedAt = question.PostedAt,
                Username = user.UserName,
                Title = question.Title,
                Id = question.Id
            }).ToList();
        }

        return new UserDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Answers = answers,
            Questions = questions
        };
    }
}