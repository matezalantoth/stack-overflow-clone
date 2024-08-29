using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(
    IQuestionRepository questionRepository,
    IUserRepository userRepository,
    IQuestionFactory questionFactory) : ControllerBase
{
    [HttpGet]
    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return questionRepository.GetQuestions();
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

            var user = await userRepository.GetUserBySessionToken(sessionToken);
            if (user == null)
            {
                throw new Exception("User could not be found");
            }

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
}