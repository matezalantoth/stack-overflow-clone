using ElProjectGrande.Extensions;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Services.QuestionServices.Factory;
using ElProjectGrande.Services.QuestionServices.Repository;
using ElProjectGrande.Services.UserServices.Repository;
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


    [HttpPost]
    public async Task<ActionResult<QuestionDTO>> PostQuestion([FromBody] NewQuestion newQuestion,
        [FromHeader(Name = "Authorization")] Guid sessionToken)
    {
        try
        {
            if (!userRepository.IsUserLoggedIn(sessionToken) || sessionToken == Guid.Empty)
            {
                return Unauthorized("That session token is expired or invalid");
            }

            var user = await userRepository.GetUserBySessionTokenOnlyQuestions(sessionToken);
            if (user == null)
            {
                throw new Exception("User could not be found");
            }

            var karma = 5;
            userRepository.UpdateKarma(user, karma);

            Console.WriteLine(user.UserName);
            var question = questionFactory.CreateQuestion(newQuestion, user);
            return Ok(questionRepository.CreateQuestion(question, user));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteQuestion([FromHeader(Name = "Authorization")] Guid sessionToken, Guid id)
    {
        try
        {
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

    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion([FromHeader(Name = "Authorization")] Guid sessionToken,
        [FromBody] UpdatedQuestion updatedQuestion, Guid id)
    {
        try
        {
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
    
    public ActionResult <MainPageQuestionDTO> GetQuestions(int startIndex)
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