using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Services.TagServices.Factory;

public class TagFactory : ITagFactory
{
    public Tag CreateTag(NewTag newTag)
    {
        return new Tag { TagName = newTag.TagName, Id = Guid.NewGuid(), Questions = [], Description = newTag.Description};
    }

    public Tag CreateNewUpdatedTagFromUpdatesAndOriginal(UpdatedTag updatedTag, Tag tag)
    {
        tag.TagName = updatedTag.TagName;
        tag.Description = updatedTag.Description;
        return tag;
    }
}