using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswersController(IAnswerRepository answerRepository) : ControllerBase
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

    // [HttpPost]
    // public ActionResult<AnswerDTO> PostNewAnswerToQuestion([FromHeader(Name = "Authorization")] Guid sessionToken,
    //     Guid questionId)
    // {
    //
    // }
}