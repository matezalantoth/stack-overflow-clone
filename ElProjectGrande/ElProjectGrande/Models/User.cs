namespace ElProjectGrande.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public byte[] Salt { get; set; }
    public DateTime DoB { get; set; }
    
    public int Karma { get; set; }
    public List<Question> Questions { get; init; } = [];
    public List<Answer> Answers { get; init; } = [];
    public Guid SessionToken { get; set; } = Guid.NewGuid();

    public override string ToString()
    {
        string stringified = "questions: [";
        foreach (Question question in Questions)
        {
            stringified += question.ToString() + Environment.NewLine;
        }

        stringified += "]";
        foreach (Answer answer in Answers)
        {
            stringified += answer.ToString() + Environment.NewLine;
        }

        return stringified;
    }
}