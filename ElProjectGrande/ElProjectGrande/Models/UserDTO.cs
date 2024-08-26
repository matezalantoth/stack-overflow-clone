namespace ElProjectGrande.Models;

public class UserDTO
{
    public Guid Id { get; set; }

    public string UserName { get; set; }
    public List<AnswerDTO> Answers { get; set; }
    public List<QuestionDTO> Questions { get; set; }
}