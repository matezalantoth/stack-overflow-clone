using System.Security.Claims;
using BackendServer.Exceptions;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Services.AnswerServices.Factory;
using BackendServer.Services.AnswerServices.Repository;
using BackendServer.Services.QuestionServices.Repository;
using BackendServer.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswersController(
    IAnswerRepository answerRepository,
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerFactory answerFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetAllAnswersForQuestion(Guid questionId)
    {
        if (!await questionRepository.CheckIfQuestionExists(questionId))
            throw new NotFoundException($"Question of id {questionId} could not be found");
        return Ok(answerRepository.GetAllAnswersByQuestionId(questionId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> PostNewAnswerToQuestion(Guid questionId, [FromBody] NewAnswer newAnswer)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? throw new BadRequestException("This token is invalid");
        var user = await userRepository.GetUserOnlyAnswers(username) ??
                   throw new NotFoundException("This user could not be found");
        var question = await questionRepository.GetQuestionById(questionId) ??
                       throw new NotFoundException("This question could not be found");

        await userRepository.CheckIfUserIsMutedOrBanned(user);
        var answer = answerFactory.CreateAnswer(newAnswer, question, user);
        await userRepository.UpdateKarma(user, 5);
        return Ok(await answerRepository.CreateAnswer(answer, user, question));
    }

    [HttpDelete("{answerId:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DeleteAnswer(Guid answerId)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? throw new BadRequestException("This token is invalid");
        var user = await userRepository.GetUserOnlyAnswers(username) ??
                   throw new NotFoundException("This user could not be foound");
        var answer = await answerRepository.GetAnswerById(answerId) ??
                     throw new NotFoundException("This answer could not be found");

        await userRepository.CheckIfUserIsMutedOrBanned(user);
        if (user.Id != answer.UserId && !User.IsInRole("Admin"))
            throw new ForbiddenException("You are not authorized to delete this answer.");
        await answerRepository.DeleteAnswer(answer, user);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> UpdateAnswer(Guid id, [FromBody] string newContent)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? throw new BadRequestException("This token is invalid");
        var user = await userRepository.GetUserOnlyAnswers(username) ??
                   throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ??
                     throw new NotFoundException("This answer could not be found");
        if (user.Id != answer.UserId && !User.IsInRole("Admin"))
            throw new ForbiddenException("You did not post this answer, you don't have permission to edit it");

        var newAnswer = answerFactory.UpdateAnswer(newContent, answer);
        return Ok(await answerRepository.UpdateAnswer(newAnswer));
    }

    [HttpPost("/accept/{answerId:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> AcceptAnswer(Guid answerId)
    {
        var answer = await answerRepository.GetAnswerById(answerId) ??
                     throw new NotFoundException($"answer of {answerId} could not be found");
        var username = User.FindFirstValue(ClaimTypes.Name) ??
                       throw new BadRequestException("This is not a valid token");
        var userId = (await userRepository.GetUserByUserName(username) ??
                      throw new NotFoundException("this user could not be found")).Id;
        if (answer.Question.UserId != userId && !User.IsInRole("Admin"))
            throw new ForbiddenException("You do not have permission to accept this answer");
        if (answer.Question.HasAccepted())
            throw new BadRequestException("This question already has an accepted answer");

        var answerUser = answer.User;
        var karma = 20;
        await userRepository.UpdateKarma(answerUser, karma);

        return Ok(await answerRepository.AcceptAnswer(answer));
    }

    [HttpPatch("{id:guid}/upvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> UpVoteAnswer(Guid id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? throw new BadRequestException("This token is invalid");
        var user = await userRepository.GetUserOnlyAnswers(username) ??
                   throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ??
                     throw new NotFoundException("This answer could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);
        var answerUser = answer.User;
        if (user.Upvotes.Contains(answer.Id))
        {
            var unVote = -1;
            await userRepository.RemoveUpvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, unVote);
            await userRepository.UpdateKarma(answerUser, unVote);

            return Content("\"Unvoted answer\"", "application/json");
        }

        if (user.Downvotes.Contains(answer.Id))
        {
            var reVote = 2;
            await userRepository.RemoveDownvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, reVote);
            await userRepository.UpdateKarma(answerUser, reVote);
            await userRepository.Upvote(user, answer.Id);

            return Content("\"Upvoted answer\"", "application/json");
        }

        var vote = 1;
        answerRepository.VoteAnswer(answer, vote);
        await userRepository.UpdateKarma(answerUser, vote);
        await userRepository.Upvote(user, answer.Id);

        return Content("\"Upvoted answer\"", "application/json");
    }

    [HttpPatch("{id:guid}/downvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DownVoteAnswer(Guid id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? throw new BadRequestException("This token is invalid");
        var user = await userRepository.GetUserOnlyAnswers(username) ??
                   throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ??
                     throw new NotFoundException("This answer could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);

        var answerUser = answer.User;

        if (user.Downvotes.Contains(answer.Id))
        {
            var unVote = 1;
            await userRepository.RemoveDownvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, unVote);
            await userRepository.UpdateKarma(answerUser, unVote);

            return Content("\"Unvoted answer\"", "application/json");
        }

        if (user.Upvotes.Contains(answer.Id))
        {
            var reVote = -2;
            await userRepository.RemoveUpvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, reVote);
            await userRepository.UpdateKarma(answerUser, reVote);
            await userRepository.Downvote(user, answer.Id);
            return Content("\"Downvoted answer\"", "application/json");
        }

        var vote = -1;
        answerRepository.VoteAnswer(answer, vote);
        await userRepository.UpdateKarma(answerUser, vote);
        await userRepository.Downvote(user, answer.Id);

        return Content("\"Downvote answer\"", "application/json");
    }

    [HttpGet("question/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> GetQuestionOfAnswer(Guid id)
    {
        var q = await answerRepository.GetQuestionOfAnswerByAnswerId(id);
        return Ok(q.Id);
    }
}