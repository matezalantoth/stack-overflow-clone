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