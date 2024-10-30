
using BackendServer.Exceptions;
using BackendServer.Extensions;
using BackendServer.Models.TagModels.DTOs;
using BackendServer.Services.AuthenticationServices.TokenService;
using BackendServer.Services.TagServices.Factory;
using BackendServer.Services.TagServices.Repository;
using BackendServer.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(ITagFactory tagFactory ,ITagRepository tagRepository, ITokenService tokenService, IUserRepository userRepository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<TagDTO> GetTags()
    {
        return tagRepository.GetTags();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagWithQuestionsDTO>> GetTagById(Guid id)
    {
        var tag = await tagRepository.GetTagById(id);
        if (tag == null) throw new NotFoundException($"Tag could not be found");

        return Ok(tag.IncludeQuestionsToDTO());
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TagDTO>> UpdateTag(
        [FromHeader(Name = "Authorization")] string sessionToken,
        [FromBody] UpdatedTag updatedTag, Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var tag = await tagRepository.GetTagById(id) ?? throw new NotFoundException($"Tag could not be found");
        var user = await userRepository.GetUserBySessionToken(sessionToken) ?? throw new NotFoundException($"User could not be found");
        if (!userRepository.IsUserAdmin(user.Id))
        {
            throw new ForbiddenException("You are not authorized to update this tag");
        }
        
        var updated = tagFactory.CreateNewUpdatedTagFromUpdatesAndOriginal(updatedTag, tag);
        
        return Ok(tagRepository.UpdateTag(updated));
    }

    [HttpGet("search/{searchTerm}")]
    [Authorize(Roles="Admin, User")]
    public ActionResult<List<TagDTO>> SearchTags(string searchTerm)
    {
        return Ok(tagRepository.SearchTags(searchTerm));
    }
}