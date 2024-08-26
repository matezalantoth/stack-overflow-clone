using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public class UserFactory(IUserVerifier userVerifier) : IUserFactory
{
    public User CreateUser(NewUser newUser)
    {
        var password = userVerifier.HashPassword(newUser.Password, out var salt);
        var user = new User
        {
            Id = Guid.NewGuid(), Name = newUser.Name, Email = newUser.Email, Password = password,
            DoB = newUser.DoB, Salt = salt
        };
        var question = new Question { Answers = [], Content = "test question", User = user };
        var answer = new Answer
        {
            Id = Guid.NewGuid(), Question = question, Content = "test answer", User = user
        };
        question.Answers.Add(answer);
        user.Questions = [question];
        user.Answers =
        [
            answer
        ];
        return user;
    }

    public UserDTO CreateUserDTO(User user)
    {
        var answers = new List<AnswerDTO>();
        var questions = new List<QuestionDTO>();

        Console.WriteLine(user.Answers);
        Console.WriteLine(user.Questions);
        if (user.Answers != null && user.Answers.Count != 0)
        {
            answers = user.Answers.Select(answer => new AnswerDTO
            {
                Content = answer.Content,
                Id = answer.Id,
                QuestionId = answer.Question.Id,
                Username = user.Name
            }).ToList();
        }

        if (user.Questions != null && user.Questions.Count != 0)
        {
            questions = user.Questions.Select(question => new QuestionDTO
            {
                Content = question.Content,
                Id = question.Id,
                Username = user.Name
            }).ToList();
        }

        return new UserDTO
        {
            Id = user.Id,
            Answers = answers,
            Questions = questions
        };
    }
}