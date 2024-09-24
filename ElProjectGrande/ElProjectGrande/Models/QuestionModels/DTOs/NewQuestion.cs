using ElProjectGrande.Models.TagModels;

namespace ElProjectGrande.Models.QuestionModels.DTOs;

public class NewQuestion
{
    public string Content { get; set; }
    public string Title { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public DateTime PostedAt { get; set; }
}