namespace ElProjectGrande.Models;

public class UserDTO
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    
    public int Karma { get; set; }
    public List<AnswerDTO> Answers { get; set; }
    public List<QuestionDTO> Questions { get; set; }
    public Guid SessionToken { get; set; }
}