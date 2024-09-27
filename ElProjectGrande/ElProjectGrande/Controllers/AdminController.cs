using ElProjectGrande.Extensions;
using ElProjectGrande.Models.AdminModels.DTOs;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.AnswerServices.Repository;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController(
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerRepository answerRepository) : ControllerBase
{
    [HttpPatch("Users/Ban/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> BanUserAsync(string userId)
    {
        return (await userRepository.BanUserById(userId)).ToDTO();
    }

    [HttpPatch("Users/Mute/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> MuteUserAsync(string userId, [FromBody] MuteRequest request)
    {
        return (await userRepository.MuteUserById(userId, request.Time)).ToDTO();
    }

    [HttpPatch("Users/UnBan/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnBanUserAsync(string userId)
    {
        return (await userRepository.UnBanUserById(userId)).ToDTO();
    }

    [HttpPatch("Users/UnMute/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> UnMuteUserAsync(string userId)
    {
        return (await userRepository.UnMuteUserById(userId)).ToDTO();
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
}