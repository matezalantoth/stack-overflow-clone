using BackendServer.Models.TagModels.DTOs;

namespace BackendServer.Models.QuestionModels.DTOs;

public class NewQuestion
{
    public string Content { get; set; }
    public string Title { get; set; }
    public List<TagForQuestion> Tags { get; set; } = [];
    public DateTime PostedAt { get; set; }
}