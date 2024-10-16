using ElProjectGrande.Extensions;
using ElProjectGrande.Models.AdminModels.DTOs;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.TagModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.AnswerServices.Repository;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.TagServices.Repository;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerRepository answerRepository,
    ITagRepository tagRepository) : ControllerBase
{
    [HttpPatch("Users/Ban/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> BanUserAsync(string username)
    {
        return (await userRepository.BanUserByUsername(username)).ToDTO();
    }

    [HttpPatch("Users/Mute/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> MuteUserAsync(string username, [FromBody] MuteRequest request)
    {
        return (await userRepository.MuteUserByUsername(username, request.Time)).ToDTO();
    }

    [HttpPatch("Users/UnBan/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnBanUserAsync(string username)
    {
        return (await userRepository.UnBanUserByUsername(username)).ToDTO();
    }

    [HttpPatch("Users/UnMute/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnMuteUserAsync(string username)
    {
        return (await userRepository.UnMuteUserByUsername(username)).ToDTO();
    }

    [HttpGet("Users/SearchByUsername/{username}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<UserDTO>> SearchByUserNameAsync(string username)
    {
        return Ok(userRepository.GetUsersWithSimilarUsernames(username));
    }

    [HttpGet("Questions/searchByTitle/{title}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<QuestionDTO>> SearchQuestionsByTitleAsync(string title)
    {
        return Ok(questionRepository.GetQuestionsByTitle(title));
    }

    [HttpGet("Questions/searchByContent/{content}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<QuestionDTO>> SearchQuestionsByContentAsync(string content)
    {
        return Ok(questionRepository.GetQuestionsByTitle(content));
    }

    [HttpGet("Answers/searchByContent/{content}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<AnswerDTO>> SearchAnswersByContentAsync(string content)
    {
        return Ok(answerRepository.GetAnswersByContent(content));
    }

    [HttpPatch("Answers/unAccept/{answerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AnswerDTO>> UnAcceptAnswer(Guid answerId)
    {
        return Ok(await answerRepository.UnAcceptAnswer(answerId));
    }

    [HttpGet("Tags/searchByTagName/{tagName}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<TagDTO>> SearchByTagNameAsync(string tagName)
    {
        return Ok(tagRepository.GetTagsByName(tagName));
    }

    [HttpGet("Tags/searchByTagDescription/{tagDescription}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<TagDTO>> SearchByTagDescriptionAsync(string tagDescription)
    {
        return Ok(tagRepository.GetTagsByDescription(tagDescription));
    }
}