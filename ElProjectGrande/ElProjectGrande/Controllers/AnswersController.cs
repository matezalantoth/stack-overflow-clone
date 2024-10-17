using ElProjectGrande.Exceptions;
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
    public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetAllAnswersForQuestion(Guid questionId)
    {
        if (!await questionRepository.CheckIfQuestionExists(questionId)) throw new NotFoundException($"Question of id {questionId} could not be found");
        return Ok(answerRepository.GetAllAnswersByQuestionId(questionId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> PostNewAnswerToQuestion(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid questionId, [FromBody] NewAnswer newAnswer)
    {

        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken) ?? throw new NotFoundException("This user could not be found");
        var question = await questionRepository.GetQuestionById(questionId) ?? throw new NotFoundException("This question could not be found");

        await userRepository.CheckIfUserIsMutedOrBanned(user);

        var answer = answerFactory.CreateAnswer(newAnswer, question, user);
        await userRepository.UpdateKarma(user, 5);
        return Ok(await answerRepository.CreateAnswer(answer, user, question));
    }

    [HttpDelete("{answerId:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DeleteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken) ?? throw new NotFoundException("This user could not be foound");
        var answer = await answerRepository.GetAnswerById(answerId) ?? throw new NotFoundException("This answer could not be found");

        await userRepository.CheckIfUserIsMutedOrBanned(user);
        if (user.Id != answer.UserId && !userRepository.IsUserAdmin(user)) return Forbid();
        await answerRepository.DeleteAnswer(answer, user);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> UpdateAnswer(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid id, [FromBody] string newContent)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken) ?? throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ?? throw new NotFoundException("This answer could not be found");
        if (user.Id != answer.UserId && !userRepository.IsUserAdmin(user)) throw new ForbiddenException("You did not post this answer, you don't have permission to edit it");

        var newAnswer = answerFactory.UpdateAnswer(newContent, answer);
        return Ok(await answerRepository.UpdateAnswer(newAnswer));
    }

    [HttpPost("/accept/{answerId:guid}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<AnswerDTO>> AcceptAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var answer = await answerRepository.GetAnswerById(answerId) ?? throw new NotFoundException($"answer of {answerId} could not be found");
        var userId = (await userRepository.GetUserBySessionToken(sessionToken) ?? throw new NotFoundException("this user could not be found")).Id;
        if (answer.Question.UserId != userId && !userRepository.IsUserAdmin(userId)) throw new ForbiddenException("You do not have permission to accept this answer");
        if (answer.Question.HasAccepted()) throw new BadRequestException("This question already has an accepted answer");

        var answerUser = answer.User;
        var karma = 20;
        await userRepository.UpdateKarma(answerUser, karma);

        return Ok(await answerRepository.AcceptAnswer(answer));
    }

    [HttpPatch("{id:guid}/upvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> UpVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken) ?? throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ?? throw new NotFoundException("This answer could not be found");
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

    [HttpPatch("{id:guid}/downvote")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DownVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken) ?? throw new NotFoundException("This user could not be found");
        var answer = await answerRepository.GetAnswerById(id) ?? throw new NotFoundException("This answer could not be found");
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

    [HttpGet("question/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> GetQuestionOfAnswer(Guid id)
    {
        var q = await answerRepository.GetQuestionOfAnswerByAnswerId(id);
        return Ok(q.Id);
    }
}