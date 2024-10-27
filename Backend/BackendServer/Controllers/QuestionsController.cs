
using BackendServer.Exceptions;
using BackendServer.Extensions;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Services.AuthenticationServices.TokenService;
using BackendServer.Services.QuestionServices.Factory;
using BackendServer.Services.QuestionServices.Repository;
using BackendServer.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(
    IQuestionRepository questionRepository,
    IUserRepository userRepository,
    IQuestionFactory questionFactory,
    ITokenService tokenService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestionById(Guid id)
    {
        var question = await questionRepository.GetQuestionById(id);
        if (question == null) throw new NotFoundException("Question could not be found");

        return Ok(question.ToDTO());
    }

    [HttpGet("trending")]
    public ActionResult<IEnumerable<QuestionDTO>> GetTrendingQuestions()
    {
        return Ok(questionRepository.GetTrendingQuestions());
    }


    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<QuestionDTO>> PostQuestion([FromHeader(Name = "Authorization")] string sessionToken,
        [FromBody] NewQuestion newQuestion)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        if (!userRepository.IsUserLoggedIn(sessionToken))
            return Unauthorized("That session token is expired or invalid");

        var user = await userRepository.GetUserBySessionTokenOnlyQuestions(sessionToken) ?? throw new NotFoundException("This user could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);

        var karma = 5;
        await userRepository.UpdateKarma(user, karma);
        
        return Ok(questionRepository.CreateQuestion(newQuestion, user));

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DeleteQuestion([FromHeader(Name = "Authorization")] string sessionToken, Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var question = await questionRepository.GetQuestionById(id) ??
                       throw new NotFoundException($"Question of id {id} could not be found!");
        var user = await userRepository.GetUserBySessionTokenOnlyQuestions(sessionToken) ??
                   throw new NotFoundException("this user could not be found!");

        if (question.User.SessionToken != user.SessionToken && !userRepository.IsUserAdmin(user))
            throw new ForbiddenException("You do not have permission to delete this question!");

        questionRepository.DeleteQuestion(question, question.User);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion(
        [FromHeader(Name = "Authorization")] string sessionToken,
        [FromBody] UpdatedQuestion updatedQuestion, Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var question = await questionRepository.GetQuestionById(id) ??
                       throw new NotFoundException($"Question of id {id} could not be found!");
        var user = await userRepository.GetUserBySessionTokenOnlyQuestions(sessionToken) ??
                   throw new NotFoundException("this user could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);
        if (question.User.Id != user.Id && !userRepository.IsUserAdmin(user))
        {
            throw new ForbiddenException("You do not have permission to update this question");
        }

        var updated = questionFactory.CreateNewUpdatedQuestionFromUpdatesAndOriginal(updatedQuestion, question);
        return Ok(questionRepository.UpdateQuestion(updated));
    }


    [HttpGet]
    public ActionResult<MainPageQuestionDTO> GetQuestions(int startIndex)
    {
        return Ok(new MainPageQuestionDTO
        {
            Questions = questionRepository.GetTenQuestion(startIndex).ToList(),
            Index = startIndex + 10
        });
    }
}