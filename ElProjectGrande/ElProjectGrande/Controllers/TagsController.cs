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
        try
        {
            var sessionToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(sessionToken))
            {
                return Unauthorized();
            }
            var tag = tagFactory.CreateTag(newTag);
            return Ok(tagRepository.CreateTag(tag));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteTag([FromHeader(Name = "Authorization")] string sessionToken, Guid id)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }
            var tag = await tagRepository.GetTagById(id);
            if (tag == null)
            {
                throw new ArgumentException($"Tag of id {id} could not be found!");
            }
            tagRepository.DeleteTag(tag);
            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}