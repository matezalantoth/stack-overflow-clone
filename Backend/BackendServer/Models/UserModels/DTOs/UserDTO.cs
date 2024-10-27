using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels.DTOs;


namespace BackendServer.Models.UserModels.DTOs;

public class UserDTO
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public List<Guid> Upvotes { get; set; }
    public List<Guid> Downvotes { get; set; }
    public int Karma { get; set; }
    public List<AnswerDTO> Answers { get; set; }
    public List<QuestionDTO> Questions { get; set; }
    public string SessionToken { get; set; }

    public bool Muted { get; set; } = false;

    public bool Banned { get; set; } = false;

    public DateTime? MutedUntil { get; set; }
}