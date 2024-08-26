namespace ElProjectGrande.Models;

public class Answer
{
    public Guid Id { get; set; }
    public string Content { get; set; }

    public Question Question { get; set; }
    public User User { get; set; }

    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }

    public override string ToString()
    {
        return Content;
    }
}