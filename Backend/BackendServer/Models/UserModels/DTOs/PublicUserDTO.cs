using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels.DTOs;


namespace BackendServer.Models.UserModels.DTOs;

public class PublicUserDTO
{
    public string UserName { get; set; }
    public int Karma { get; set; }
    public List<AnswerDTO> Answers { get; set; }
    public List<QuestionDTO> Questions { get; set; }

    public bool Muted { get; set; } = false;

    public bool Banned { get; set; } = false;

    public DateTime? MutedUntil { get; set; }
}