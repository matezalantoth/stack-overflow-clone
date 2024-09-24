using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Extensions;

public static class TagExtensions
{
    public static TagDTO ToDTO(this Tag tag)
    {
        return new TagDTO
        {
            TagName = tag.TagName, Id = tag.Id
        };
    }
}