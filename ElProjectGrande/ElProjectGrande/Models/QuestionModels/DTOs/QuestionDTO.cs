namespace ElProjectGrande.Models.QuestionModels.DTOs;

public class QuestionDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Username { get; set; }
    public string Title { get; set; }
    public DateTime PostedAt { get; set; }
    public bool HasAccepted { get; set; }
}