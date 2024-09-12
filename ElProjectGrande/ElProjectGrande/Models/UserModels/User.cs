using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.QuestionModels;

namespace ElProjectGrande.Models.UserModels;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public byte[] Salt { get; set; }
    public DateTime DoB { get; set; }
    public List<Guid> Upvotes { get; set; } = [];
    public List<Guid> Downvotes { get; set; } = [];
    public int Karma { get; set; }
    public List<Question> Questions { get; init; } = [];
    public List<Answer> Answers { get; init; } = [];
    public Guid SessionToken { get; set; } = Guid.NewGuid();
}