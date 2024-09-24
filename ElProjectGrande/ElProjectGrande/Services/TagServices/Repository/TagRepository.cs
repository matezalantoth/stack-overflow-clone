using ElProjectGrande.Data;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.TagModels.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Services.TagServices.Repository;

public class TagRepository(ApiDbContext context) : ITagRepository
{
    public async Task<bool> CheckIfTagExists(Guid id)
    {
        return await context.Tags.FirstOrDefaultAsync(t => t.Id == id) != null;
    }

    public IEnumerable<TagDTO> GetTags()
    {
        return context.Tags.Select(t => t.ToDTO());
    }

    public Task<Tag?> GetTagById(Guid id)
    {
        return context.Tags
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public TagDTO CreateTag(Tag tag)
    {
        context.Tags.Add(tag);
        context.SaveChanges();
        return new TagDTO
        {
            Id = tag.Id,
            TagName = tag.TagName,
        };
    }

    public void DeleteTag(Tag tag)
    {
        context.Tags.Remove(tag);
        context.SaveChanges();
    }
    
}