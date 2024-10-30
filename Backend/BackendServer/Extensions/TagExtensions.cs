using BackendServer.Models.TagModels;
using BackendServer.Models.TagModels.DTOs;

namespace BackendServer.Extensions;

public static class TagExtensions
{
    public static TagDTO ToDTO(this Tag tag)
    {
        return new TagDTO
        {
            TagName = tag.TagName, Id = tag.Id, Description = tag.Description
        };
    }
    
    public static TagWithQuestionsDTO IncludeQuestionsToDTO(this Tag tag)
    {
        return new TagWithQuestionsDTO
        {
            TagName = tag.TagName, Id = tag.Id, Description = tag.Description, Questions = tag.Questions.Select(q => q.ToDTO()).ToList()
        };
    }
}