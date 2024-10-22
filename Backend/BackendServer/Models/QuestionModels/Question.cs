using BackendServer.Models.AnswerModels;
using BackendServer.Models.TagModels;
using BackendServer.Models.UserModels;


namespace BackendServer.Models.QuestionModels;

public class Question
{
    public string Content { get; set; }
    public string Title { get; set; }
    public List<Answer> Answers { get; set; } = new List<Answer>();
    public List<Tag> Tags { get; set; }
    public Guid Id { get; set; }
    public User User { get; set; }
    public DateTime PostedAt { get; set; }
    public string UserId { get; set; }

    public bool HasAccepted()
    {
        return Answers.Any(answer => answer.Accepted);
    }


    public override string ToString()
    {
        return Content;
    }
}