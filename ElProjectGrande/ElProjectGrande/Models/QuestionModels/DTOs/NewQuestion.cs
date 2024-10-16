using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Models.QuestionModels.DTOs;

public class NewQuestion
{
    public string Content { get; set; }
    public string Title { get; set; }
    public List<TagForQuestion> Tags { get; set; } = [];
    public DateTime PostedAt { get; set; }
}