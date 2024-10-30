using BackendServer.Exceptions;
using BackendServer.Extensions;
using BackendServer.Models.TagModels.DTOs;
using BackendServer.Services.TagServices.Factory;
using BackendServer.Services.TagServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(
    ITagFactory tagFactory,
    ITagRepository tagRepository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<TagDTO> GetTags()
    {
        return tagRepository.GetTags();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDTO>> GetTagById(Guid id)
    {
        var tag = await tagRepository.GetTagById(id);
        if (tag == null) throw new NotFoundException("Tag could not be found");

        return Ok(tag.ToDTO());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TagDTO>> CreateTag([FromBody] NewTag newTag)
    {
        var tag = tagFactory.CreateTag(newTag);
        return Ok(tagRepository.CreateTag(tag));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTag(Guid id)
    {
        var tag = await tagRepository.GetTagById(id) ??
                  throw new NotFoundException($"Tag of id {id} could not be found!");
        tagRepository.DeleteTag(tag);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TagDTO>> UpdateTag(
        [FromBody] UpdatedTag updatedTag, Guid id)
    {
        var tag = await tagRepository.GetTagById(id) ?? throw new NotFoundException("Tag could not be found");
        var updated = tagFactory.CreateNewUpdatedTagFromUpdatesAndOriginal(updatedTag, tag);

        return Ok(tagRepository.UpdateTag(updated));
    }

    [HttpGet("search/{searchTerm}")]
    [Authorize(Roles = "Admin, User")]
    public ActionResult<List<TagDTO>> SearchTags(string searchTerm)
    {
        return Ok(tagRepository.SearchTags(searchTerm));
    }
}