using BackendServer.Models.QuestionModels.DTOs;

namespace BackendServer.Models.TagModels.DTOs;

public class TagWithQuestionsDTO
{
    public Guid Id { get; set; }
    public string TagName { get; set; }
    public string Description { get; set; }
    public List<QuestionDTO> Questions { get; set; }
}