using ElProjectGrande.Models;

namespace ElProjectGrande.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {
        Console.WriteLine($"UserName: {user.UserName}");
        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"SessionToken: {user.SessionToken}");
        Console.WriteLine($"Answers: {user.Answers}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Questions: {user.Questions}");
        Console.WriteLine($"Karma: {user.Karma}");


        return new UserDTO
        {
            UserName = user.UserName, Name = user.Name, SessionToken = user.SessionToken,
            Answers = user.Answers.Select(answer => answer.ToDTO()).ToList(),
            Email = user.Email, Questions = user.Questions.Select(question => question.ToDTO()).ToList(),
            Karma = user.Karma
        };
    }
}