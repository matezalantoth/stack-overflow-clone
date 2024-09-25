using ElProjectGrande.Extensions;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Services.QuestionServices.Factory;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(
    IQuestionRepository questionRepository,
    IUserRepository userRepository,
    IQuestionFactory questionFactory) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestionById(Guid id)
    {
        try
        {
            var question = await questionRepository.GetQuestionById(id);
            if (question == null)
            {
                throw new ArgumentException();
            }

            return Ok(question.ToDTO());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound("This question could not be found");
        }
    }

    [HttpGet("trending")]
    public ActionResult<IEnumerable<QuestionDTO>> GetTrendingQuestions()
    {
        return Ok(questionRepository.GetTrendingQuestions());
    }


    [HttpPost, Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<QuestionDTO>> PostQuestion([FromBody] NewQuestion newQuestion)
    {
        try
        {
            var sessionToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(sessionToken))
            {
                return Unauthorized();
            }

            Console.WriteLine(userRepository.IsUserLoggedIn(sessionToken));

            if (!userRepository.IsUserLoggedIn(sessionToken) || sessionToken == String.Empty)
            {
                return Unauthorized("That session token is expired or invalid");
            }

            var user = await userRepository.GetUserBySessionTokenOnlyQuestions(sessionToken);
            if (user == null)
            {
                throw new Exception("User could not be found");
            }

            var karma = 5;
            await userRepository.UpdateKarma(user, karma);

            Console.WriteLine(user.UserName);
            var question = questionFactory.CreateQuestion(newQuestion, user);
            return Ok(questionRepository.CreateQuestion(question, user));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}"), Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> DeleteQuestion([FromHeader(Name = "Authorization")] string sessionToken, Guid id)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var question = await questionRepository.GetQuestionById(id);
            if (question == null)
            {
                throw new ArgumentException($"Question of id {id} could not be found!");
            }

            if (question.User.SessionToken != sessionToken)
            {
                return Unauthorized("You do not have permission to delete this question");
            }

            questionRepository.DeleteQuestion(question, question.User);
            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}"), Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion(
        [FromHeader(Name = "Authorization")] string sessionToken,
        [FromBody] UpdatedQuestion updatedQuestion, Guid id)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
            var question = await questionRepository.GetQuestionById(id);
            if (question == null)
            {
                throw new ArgumentException($"Question of id {id} could not be found!");
            }

            if (question.User.SessionToken != sessionToken)
            {
                return Unauthorized("You do not have permission to update this question");
            }

            var updated = questionFactory.CreateNewUpdatedQuestionFromUpdatesAndOriginal(updatedQuestion, question);
            return Ok(questionRepository.UpdateQuestion(updated));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }


    [HttpGet]
    public ActionResult<MainPageQuestionDTO> GetQuestions(int startIndex)
    {
        var questions = questionRepository.GetTenQuestion(startIndex).ToList();
        startIndex += 10;


        return Ok(new MainPageQuestionDTO
        {
            Questions = questions,
            Index = startIndex
        });
    }
}