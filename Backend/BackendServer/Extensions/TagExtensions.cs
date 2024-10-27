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
}