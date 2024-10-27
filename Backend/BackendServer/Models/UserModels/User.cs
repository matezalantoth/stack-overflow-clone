using BackendServer.Models.AnswerModels;
using BackendServer.Models.QuestionModels;
using Microsoft.AspNetCore.Identity;

namespace BackendServer.Models.UserModels;

public class User : IdentityUser
{
    public string Name { get; set; }
    public DateTime DoB { get; set; }
    public List<Guid> Upvotes { get; set; } = [];
    public List<Guid> Downvotes { get; set; } = [];
    public int Karma { get; set; } = 0;
    public List<Question> Questions { get; init; } = [];
    public List<Answer> Answers { get; init; } = [];
    public string SessionToken { get; set; } = string.Empty;
    public bool Muted { get; set; } = false;
    public bool Banned { get; set; } = false;
    public DateTime? MutedUntil { get; set; }
}