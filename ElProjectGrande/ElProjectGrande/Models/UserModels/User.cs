using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.QuestionModels;
using Microsoft.AspNetCore.Identity;

namespace ElProjectGrande.Models.UserModels;

public class User : IdentityUser
{
    public string Name { get; set; }
    public DateTime DoB { get; set; }
    public List<Guid> Upvotes { get; set; } = [];
    public List<Guid> Downvotes { get; set; } = [];
    public int Karma { get; set; }
    public List<Question> Questions { get; init; } = [];
    public List<Answer> Answers { get; init; } = [];
    public string SessionToken { get; set; } = string.Empty;
}