using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Services.TagServices.Factory;

public class TagFactory : ITagFactory
{
    public Tag CreateTag(NewTag newTag)
    {
        return new Tag { TagName = newTag.TagName, Id = Guid.NewGuid(), Questions = []};
    }
}