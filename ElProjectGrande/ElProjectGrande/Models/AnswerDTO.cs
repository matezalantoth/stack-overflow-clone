namespace ElProjectGrande.Models;

public class AnswerDTO
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public string Username { get; set; }
}