using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;

namespace ElProjectGrande.Services.TagServices.Repository;

public interface ITagRepository
{
    public Task<bool> CheckIfTagExists(Guid id);

    public IEnumerable<TagDTO> GetTags();
    
    public Task<Tag?> GetTagById(Guid id);
    
    public TagDTO CreateTag(Tag tag);
    
    public TagDTO UpdateTag(Tag tag);
    
    public void DeleteTag(Tag tag);

    public IEnumerable<TagDTO> GetTagsByName(string nameSubstring);

    public IEnumerable<TagDTO> GetTagsByDescription(string descriptionSubstring);
}