using ElProjectGrande.Models;
using ElProjectGrande.Services;
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
    public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetAllAnswersForQuestion(Guid questionId)
    {
        try
        {
            var question = await questionRepository.GetQuestionById(questionId);
            if (question == null)
            {
                throw new ArgumentException($"Question of id {questionId} could not be found");
            }

            return Ok(answerRepository.GetAllAnswersFromQuestion(question));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AnswerDTO>> PostNewAnswerToQuestion(
        [FromHeader(Name = "Authorization")] Guid sessionToken,
        Guid questionId, [FromBody] NewAnswer newAnswer)
    {
        try
        {
            var user = await userRepository.GetUserBySessionToken(sessionToken);
            var question = await questionRepository.GetQuestionById(questionId);
            if (user == null || question == null)
            {
                return NotFound("This user or question could not be found");
            }

            var answer = answerFactory.CreateAnswer(newAnswer, question, user);
            
            var karma = 5;
            userRepository.UpdateKarma(user, karma);
            
            return Ok(await answerRepository.CreateAnswer(answer, user, question));
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    [HttpDelete("{answerId}")]
    public async Task<ActionResult> DeleteAnswer([FromHeader(Name = "Authorization")] Guid sessionToken, Guid answerId)
    {
        try
        {
            var user = await userRepository.GetUserBySessionToken(sessionToken);
            var answer = await answerRepository.GetAnswerById(answerId);
            if (user == null || answer == null)
            {
                return NotFound("This answer or user could not be found!");
            }

            if (user.Id != answer.UserId)
            {
                return Forbid();
            }

            answerRepository.DeleteAnswer(answer, user);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AnswerDTO>> UpdateAnswer([FromHeader(Name = "Authorization")] Guid sessionToken,
        Guid id, [FromBody] string newContent)
    {
        try
        {
            var user = await userRepository.GetUserBySessionToken(sessionToken);
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

    [HttpPost("/accept/{answerId}")]
    public async Task<ActionResult<AnswerDTO>> AcceptAnswer([FromHeader(Name = "Authorization")] Guid sessionToken,
        Guid answerId)
    {
        try
        {
            var answer = await answerRepository.GetAnswerById(answerId);
            var user = await userRepository.GetUserBySessionToken(sessionToken);
            if (answer == null || user == null)
            {
                return NotFound("this answer or user could not be found");
            }

            if (answer.Question.UserId != user.Id)
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

    [HttpPatch("{id}/upvote")]
    public async Task<ActionResult> UpVoteAnswer([FromHeader(Name = "Authorization")] Guid sessionToken,
        Guid id)
    {
        try
        {
            var user = await userRepository.GetUserBySessionToken(sessionToken);
            var answer = await answerRepository.GetAnswerById(id);
            if (user == null || answer == null)
            {
                return NotFound("This user or answer could not be found");
            }

            if (user.Id != answer.UserId)
            {
                return Forbid();
            }

            if (user.Upvotes.Contains(answer.Id))
            {
                return BadRequest("You can't upvote the same answer twice");
            }

            if (user.Downvotes.Contains(answer.Id))
            {
                userRepository.RemoveDownvote(user, answer.Id);
            }
            var vote = 1;
            answerRepository.VoteAnswer(answer, vote);
            var answerUser = answer.User;
            userRepository.UpdateKarma(answerUser, vote);
            userRepository.Upvote(user, answer.Id);

            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }

    [HttpPatch("{id}/downvote")]
    public async Task<ActionResult> DownVoteAnswer([FromHeader(Name = "Authorization")] Guid sessionToken,
        Guid id)
    {
        try
        {
            var user = await userRepository.GetUserBySessionToken(sessionToken);
            var answer = await answerRepository.GetAnswerById(id);
            if (user == null || answer == null)
            {
                return NotFound("This user or answer could not be found");
            }

            if (user.Id != answer.UserId)
            {
                return Forbid();
            }
            
            if (user.Downvotes.Contains(answer.Id))
            {
                return BadRequest("You can't downvote the same answer twice");
            }

            if (user.Upvotes.Contains(answer.Id))
            {
                userRepository.RemoveUpvote(user, answer.Id);
            }

            var vote = -1;
            answerRepository.VoteAnswer(answer, vote);
            var answerUser = answer.User;
            userRepository.UpdateKarma(answerUser, vote);
            userRepository.Downvote(user, answer.Id);

            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500);
        }
    }
    
}