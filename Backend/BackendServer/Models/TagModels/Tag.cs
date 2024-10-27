using BackendServer.Models.QuestionModels;

namespace BackendServer.Models.TagModels;

public class Tag
{
    public Guid Id { get; set; }
    
    public string TagName { get; set; }
    
    public string Description { get; set; }
    
    public List<Question> Questions { get; set; } = new List<Question>();
}