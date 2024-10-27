using BackendServer.Models.TagModels;
using BackendServer.Models.TagModels.DTOs;

namespace BackendServer.Services.TagServices.Factory;

public interface ITagFactory
{
    public Tag CreateTag(NewTag newTag);
    
    public Tag CreateNewUpdatedTagFromUpdatesAndOriginal(UpdatedTag updatedTag, Tag tag);
}