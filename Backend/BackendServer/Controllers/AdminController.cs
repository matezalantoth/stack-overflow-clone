using BackendServer.Extensions;
using BackendServer.Models.AdminModels.DTOs;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Models.TagModels.DTOs;
using BackendServer.Models.UserModels.DTOs;
using BackendServer.Services.AnswerServices.Repository;
using BackendServer.Services.QuestionServices.Repository;
using BackendServer.Services.TagServices.Repository;
using BackendServer.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerRepository answerRepository,
    ITagRepository tagRepository) : ControllerBase
{
    //  admin/users/ban/:username
    [HttpPatch("Users/Ban/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> BanUserAsync(string username)
    {
        return (await userRepository.BanUserByUsername(username)).ToDTO();
    }

    //  admin/users/mute/:username
    [HttpPatch("Users/Mute/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> MuteUserAsync(string username, [FromBody] MuteRequest request)
    {
        return Ok((await userRepository.MuteUserByUsername(username, request.Time)).ToDTO());
    }

    //  admin/users/unban/:username
    [HttpPatch("Users/UnBan/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnBanUserAsync(string username)
    {
        return (await userRepository.UnBanUserByUsername(username)).ToDTO();
    }

    //  admin/users/unmute/:username
    [HttpPatch("Users/UnMute/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnMuteUserAsync(string username)
    {
        return (await userRepository.UnMuteUserByUsername(username)).ToDTO();
    }

    //  admin/users/searchByUsername/:username
    [HttpGet("Users/SearchByUsername/{username}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<string>> SearchByUserNameAsync(string username)
    {
        return Ok(userRepository.GetUsersWithSimilarUsernames(username));
    }

    //  admin/questions/searchByTitle/:title
    [HttpGet("Questions/searchByTitle/{title}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<QuestionDTO>> SearchQuestionsByTitleAsync(string title)
    {
        return Ok(questionRepository.GetQuestionsByTitle(title));
    }

    //  admin/questions/searchByContent/:content
    [HttpGet("Questions/searchByContent/{content}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<QuestionDTO>> SearchQuestionsByContentAsync(string content)
    {
        return Ok(questionRepository.GetQuestionsByContent(content));
    }

    //  admin/answers/searchByContent/:content
    [HttpGet("Answers/searchByContent/{content}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<AnswerDTO>> SearchAnswersByContentAsync(string content)
    {
        return Ok(answerRepository.GetAnswersByContent(content));
    }

    //  admin/answers/unAccept/:answerId
    [HttpPatch("Answers/unAccept/{answerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AnswerDTO>> UnAcceptAnswer(Guid answerId)
    {
        return Ok(await answerRepository.UnAcceptAnswer(answerId));
    }

    //  admin/tags/searchByTagName/:tagName
    [HttpGet("Tags/searchByTagName/{tagName}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<TagDTO>> SearchByTagNameAsync(string tagName)
    {
        return Ok(tagRepository.GetTagsByName(tagName));
    }

    //  admin/tags/searchByTagDescription/:tagDescription
    [HttpGet("Tags/searchByTagDescription/{tagDescription}")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<TagDTO>> SearchByTagDescriptionAsync(string tagDescription)
    {
        return Ok(tagRepository.GetTagsByDescription(tagDescription));
    }
}