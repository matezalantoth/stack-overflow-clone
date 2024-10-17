using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Services.TagServices.Factory;

public interface ITagFactory
{
    public Tag CreateTag(NewTag newTag);
    
    public Tag CreateNewUpdatedTagFromUpdatesAndOriginal(UpdatedTag updatedTag, Tag tag);
}