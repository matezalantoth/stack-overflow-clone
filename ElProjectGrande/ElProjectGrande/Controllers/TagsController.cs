using ElProjectGrande.Exceptions;
using ElProjectGrande.Models.TagModels.DTOs;
using ElProjectGrande.Services.TagServices.Factory;
using ElProjectGrande.Services.TagServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(ITagFactory tagFactory ,ITagRepository tagRepository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<TagDTO> GetTags()
    {
        return tagRepository.GetTags();
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<ActionResult<TagDTO>> CreateTag([FromBody] NewTag newTag)
    {
            var tag = tagFactory.CreateTag(newTag);
            return Ok(tagRepository.CreateTag(tag));
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTag(Guid id)
    {
            var tag = await tagRepository.GetTagById(id) ?? throw new NotFoundException($"Tag of id {id} could not be found!");
            tagRepository.DeleteTag(tag);
            return NoContent();
    }
}