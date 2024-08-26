namespace ElProjectGrande.Models;

public class Question
{
    public string Content { get; set; }
    public List<Answer> Answers { get; set; }
    public Guid Id { get; set; }
    public User User { get; set; }

    public Guid UserId { get; set; }

    public override string ToString()
    {
        return Content;
    }
}