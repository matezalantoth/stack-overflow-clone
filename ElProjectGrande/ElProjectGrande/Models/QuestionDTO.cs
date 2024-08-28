namespace ElProjectGrande.Models;

public class QuestionDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Username { get; set; }
    public string Title { get; set; }
    public DateTime PostedAt { get; set; }
}