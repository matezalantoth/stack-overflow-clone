using BackendServer.Data;
using BackendServer.Exceptions;
using BackendServer.Extensions;
using BackendServer.Models.TagModels;
using BackendServer.Models.TagModels.DTOs;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Services.TagServices.Repository;

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
            .ThenInclude(q => q.User)
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
            Description = tag.Description
        };
    }

    public void DeleteTag(Tag tag)
    {
        context.Tags.Remove(tag);
        context.SaveChanges();
    }

    public IEnumerable<TagDTO> GetTagsByName(string nameSubstring)
    {
        var closestName = Process.ExtractSorted(nameSubstring, context.Tags.Select(t => t.TagName).ToArray())
            .Select(res => res.Value)
            .Take(10);

        var tags = context.Tags
            .Include(t => t.Questions);
        
        return closestName
            .Select(name => tags.FirstOrDefault(t => t.TagName == name))
            .Select(t => t?.ToDTO() ?? throw new NotFoundException("Tag not found"));
    }

    public IEnumerable<TagDTO> GetTagsByDescription(string descriptionSubstring)
    {
        var closestDescription = Process
            .ExtractSorted(descriptionSubstring, context.Tags.Select(t => t.Description).ToArray())
            .Select(res => res.Value)
            .Take(10);
        
        var tags = context.Tags
            .Include(t => t.Questions);
        
        return closestDescription
            .Select(description => tags.FirstOrDefault(t => t.Description == description))
            .Select(t => t?.ToDTO() ?? throw new NotFoundException("Tag not found"));
    }

    public IEnumerable<TagDTO> SearchTags(string searchTerm)
    {
        List<string> searchable = context.Tags.Select(t => t.TagName).ToList();
        searchable.AddRange(context.Tags.Select(t => t.Description));
        var closest = Process.ExtractSorted(searchTerm, searchable.ToArray())
            .Select(res => res.Value)
            .Take(10);

        var tags = context.Tags
            .Include(t => t.Questions);

        return closest.Select(res => tags.FirstOrDefault(t => t.TagName == res || t.Description == res)).Select(t => t?.ToDTO() ?? throw new NotFoundException("Tag not found"));
    }

    public TagDTO UpdateTag(Tag tag)
    {
        context.Update(tag);
        context.SaveChanges();
        return new TagDTO
        {
            Id = tag.Id, TagName = tag.TagName, Description = tag.Description
        };
    }
}