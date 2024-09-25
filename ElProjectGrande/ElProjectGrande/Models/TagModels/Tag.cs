using ElProjectGrande.Models.QuestionModels;

namespace ElProjectGrande.Models.TagModels;

public class Tag
{
    public Guid Id { get; set; }
    
    public string TagName { get; set; }
    
    public List<Question> Questions { get; set; } = new List<Question>();
}