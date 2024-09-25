using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Services.AnswerServices.Factory;
using ElProjectGrande.Services.AnswerServices.Repository;
using ElProjectGrande.Services.AuthenticationServices.TokenService;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswersController(
    IAnswerRepository answerRepository,
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerFactory answerFactory,
    ITokenService tokenService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnswersOfQuestionDTO>>> GetAllAnswersForQuestion(Guid questionId)
    {
        try
        {
            if (!await questionRepository.CheckIfQuestionExists(questionId))
                throw new ArgumentException($"Question of id {questionId} could not be found");

            return Ok(answerRepository.GetAllAnswersByQuestionId(questionId));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> PostNewAnswerToQuestion(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid questionId, [FromBody] NewAnswer newAnswer)
    {
        try
        {
            try
            {
                sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Unauthorized();
            }

            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var question = await questionRepository.GetQuestionById(questionId);

            if (user == null || question == null) return NotFound("This user or question could not be found");

            try
            {
                await userRepository.CheckIfUserIsMutedOrBanned(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Unauthorized();
            }

            var answer = answerFactory.CreateAnswer(newAnswer, question, user);
            await userRepository.UpdateKarma(user, 5);
            return Ok(await answerRepository.CreateAnswer(answer, user, question));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpDelete("{answerId}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DeleteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
        var answer = await answerRepository.GetAnswerById(answerId);
        if (user == null || answer == null) return NotFound("This answer or user could not be found!");

        await userRepository.CheckIfUserIsMutedOrBanned(user);

        if (user.Id != answer.UserId) return Forbid();
        await answerRepository.DeleteAnswer(answer, user);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswersOfQuestionDTO>> UpdateAnswer(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid id, [FromBody] string newContent)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
        var answer = await answerRepository.GetAnswerById(id);
        if (user == null || answer == null) return NotFound("This user or answer could not be found");

        if (user.Id != answer.UserId) return Forbid();

        var newAnswer = answerFactory.UpdateAnswer(newContent, answer);
        return Ok(await answerRepository.UpdateAnswer(newAnswer));
    }

    [HttpPost("/accept/{answerId}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> AcceptAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var answer = await answerRepository.GetAnswerById(answerId);
        var userId = (await userRepository.GetUserBySessionToken(sessionToken))?.Id;
        if (answer == null || userId == string.Empty) return NotFound("this answer or user could not be found");

        if (answer.Question.UserId != userId) return Forbid();

        if (answer.Question.HasAccepted()) return BadRequest("This question already has an accepted answer");

        var answerUser = answer.User;
        var karma = 20;
        await userRepository.UpdateKarma(answerUser, karma);

        return Ok(await answerRepository.AcceptAnswer(answer));
    }

    [HttpPatch("{id}/upvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> UpVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
        var answer = await answerRepository.GetAnswerById(id);
        if (user == null || answer == null) return NotFound("This user or answer could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);
        var answerUser = answer.User;

        if (user.Upvotes.Contains(answer.Id))
        {
            var unVote = -1;
            await userRepository.RemoveUpvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, unVote);
            await userRepository.UpdateKarma(answerUser, unVote);

            return Ok("Unvoted answer");
        }

        if (user.Downvotes.Contains(answer.Id))
        {
            var reVote = 2;
            await userRepository.RemoveDownvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, reVote);
            await userRepository.UpdateKarma(answerUser, reVote);
            await userRepository.Upvote(user, answer.Id);

            return Ok("Upvoted answer");
        }

        var vote = 1;
        answerRepository.VoteAnswer(answer, vote);
        await userRepository.UpdateKarma(answerUser, vote);
        await userRepository.Upvote(user, answer.Id);

        return Ok("Upvoted answer");
    }

    [HttpPatch("{id}/downvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DownVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
        var answer = await answerRepository.GetAnswerById(id);
        if (user == null || answer == null) return NotFound("This user or answer could not be found");
        await userRepository.CheckIfUserIsMutedOrBanned(user);

        var answerUser = answer.User;

        if (user.Downvotes.Contains(answer.Id))
        {
            var unVote = 1;
            await userRepository.RemoveDownvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, unVote);
            await userRepository.UpdateKarma(answerUser, unVote);

            return Ok("Unvoted answer");
        }

        if (user.Upvotes.Contains(answer.Id))
        {
            var reVote = -2;
            await userRepository.RemoveUpvote(user, answer.Id);
            answerRepository.VoteAnswer(answer, reVote);
            await userRepository.UpdateKarma(answerUser, reVote);
            await userRepository.Downvote(user, answer.Id);
            return Ok("Downvoted answer");
        }

        var vote = -1;
        answerRepository.VoteAnswer(answer, vote);
        await userRepository.UpdateKarma(answerUser, vote);
        await userRepository.Downvote(user, answer.Id);

        return Ok("Downvoted answer");
    }
}