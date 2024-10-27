using BackendServer.Models.QuestionModels.DTOs;

namespace BackendServer.Models.AnswerModels.DTOs;

public class AdminAnswerDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public UpdatedQuestion Question { get; set; }
}