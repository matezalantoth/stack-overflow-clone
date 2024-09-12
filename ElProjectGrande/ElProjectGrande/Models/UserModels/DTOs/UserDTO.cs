using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;

namespace ElProjectGrande.Models.UserModels.DTOs;

public class UserDTO
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public List<Guid> Upvotes { get; set; }
    public List<Guid> Downvotes { get; set; }
    public int Karma { get; set; }
    public List<AnswerDTO> Answers { get; set; }
    public List<QuestionDTO> Questions { get; set; }
    public Guid SessionToken { get; set; }
}