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
    public ActionResult<IEnumerable<AnswerDTO>> GetAllAnswersForQuestion(Guid id)
    {
        try
        {
            return Ok(answerRepository.GetAllAnswersFromQuestion(id));
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
            return Ok(await answerRepository.CreateAnswer(answer, user, question));
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}