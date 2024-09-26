using ElProjectGrande.Models.QuestionModels.DTOs;

namespace ElProjectGrande.Models.AnswerModels.DTOs;

public class AdminAnswerDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public UpdatedQuestion Question { get; set; }
}