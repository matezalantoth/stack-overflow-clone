using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Services.AnswerServices.Factory;
using ElProjectGrande.Services.AnswerServices.Repository;
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
    IAnswerFactory answerFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnswersOfQuestionDTO>>> GetAllAnswersForQuestion(Guid questionId)
    {
        try
        {
            if (!await questionRepository.CheckIfQuestionExists(questionId))
            {
                throw new ArgumentException($"Question of id {questionId} could not be found");
            }

            return Ok(answerRepository.GetAllAnswersByQuestionId(questionId));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<AnswerDTO>> PostNewAnswerToQuestion(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid questionId, [FromBody] NewAnswer newAnswer)
    {
        try
        {
            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var question = await questionRepository.GetQuestionById(questionId);
            if (user == null || question == null)
            {
                return NotFound("This user or question could not be found");
            }

            var answer = answerFactory.CreateAnswer(newAnswer, question, user);
            userRepository.UpdateKarma(user, 5);
            return Ok(await answerRepository.CreateAnswer(answer, user, question));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpDelete("{answerId}"), Authorize]
    public async Task<ActionResult> DeleteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var answer = await answerRepository.GetAnswerById(answerId);
            if (user == null || answer == null)
            {
                return NotFound("This answer or user could not be found!");
            }

            if (user.Id != answer.UserId)
            {
                return Forbid();
            }

            await answerRepository.DeleteAnswer(answer, user);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpPut("{id}"), Authorize]
    public async Task<ActionResult<AnswersOfQuestionDTO>> UpdateAnswer(
        [FromHeader(Name = "Authorization")] string sessionToken,
        Guid id, [FromBody] string newContent)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var answer = await answerRepository.GetAnswerById(id);
            if (user == null || answer == null)
            {
                return NotFound("This user or answer could not be found");
            }

            if (user.Id != answer.UserId)
            {
                return Forbid();
            }

            var newAnswer = answerFactory.UpdateAnswer(newContent, answer);
            return Ok(await answerRepository.UpdateAnswer(newAnswer));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpPost("/accept/{answerId}"), Authorize]
    public async Task<ActionResult<AnswerDTO>> AcceptAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid answerId)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var answer = await answerRepository.GetAnswerById(answerId);
            var userId = (await userRepository.GetUserBySessionToken(sessionToken))?.Id;
            if (answer == null || userId == String.Empty)
            {
                return NotFound("this answer or user could not be found");
            }

            if (answer.Question.UserId != userId)
            {
                return Forbid();
            }

            if (answer.Question.HasAccepted())
            {
                return BadRequest("This question already has an accepted answer");
            }

            var answerUser = answer.User;
            var karma = 20;
            userRepository.UpdateKarma(answerUser, karma);
            return Ok(await answerRepository.AcceptAnswer(answer));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPatch("{id}/upvote"), Authorize]
    public async Task<ActionResult> UpVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var answer = await answerRepository.GetAnswerById(id);
            if (user == null || answer == null)
            {
                return NotFound("This user or answer could not be found");
            }

            var answerUser = answer.User;

            if (user.Upvotes.Contains(answer.Id))
            {
                var unVote = -1;
                userRepository.RemoveUpvote(user, answer.Id);
                answerRepository.VoteAnswer(answer, unVote);
                userRepository.UpdateKarma(answerUser, unVote);

                return Ok("Unvoted answer");
            }

            if (user.Downvotes.Contains(answer.Id))
            {
                var reVote = 2;
                userRepository.RemoveDownvote(user, answer.Id);
                answerRepository.VoteAnswer(answer, reVote);
                userRepository.UpdateKarma(answerUser, reVote);
                userRepository.Upvote(user, answer.Id);

                return Ok("Upvoted answer");
            }

            var vote = 1;
            answerRepository.VoteAnswer(answer, vote);
            userRepository.UpdateKarma(answerUser, vote);
            userRepository.Upvote(user, answer.Id);

            return Ok("Upvoted answer");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpPatch("{id}/downvote"), Authorize]
    public async Task<ActionResult> DownVoteAnswer([FromHeader(Name = "Authorization")] string sessionToken,
        Guid id)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var user = await userRepository.GetUserBySessionTokenOnlyAnswers(sessionToken);
            var answer = await answerRepository.GetAnswerById(id);
            if (user == null || answer == null)
            {
                return NotFound("This user or answer could not be found");
            }

            var answerUser = answer.User;

            if (user.Downvotes.Contains(answer.Id))
            {
                var unVote = 1;
                userRepository.RemoveDownvote(user, answer.Id);
                answerRepository.VoteAnswer(answer, unVote);
                userRepository.UpdateKarma(answerUser, unVote);

                return Ok("Unvoted answer");
            }

            if (user.Upvotes.Contains(answer.Id))
            {
                var reVote = -2;
                userRepository.RemoveUpvote(user, answer.Id);
                answerRepository.VoteAnswer(answer, reVote);
                userRepository.UpdateKarma(answerUser, reVote);
                userRepository.Downvote(user, answer.Id);
                return Ok("Downvoted answer");
            }

            var vote = -1;
            answerRepository.VoteAnswer(answer, vote);
            userRepository.UpdateKarma(answerUser, vote);
            userRepository.Downvote(user, answer.Id);

            return Ok("Downvoted answer");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }
}