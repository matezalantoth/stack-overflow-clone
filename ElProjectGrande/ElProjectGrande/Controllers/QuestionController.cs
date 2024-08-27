using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController(
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

            var question = questionFactory.CreateQuestion(newQuestion, user);
            return Ok(questionRepository.CreateQuestion(question));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}