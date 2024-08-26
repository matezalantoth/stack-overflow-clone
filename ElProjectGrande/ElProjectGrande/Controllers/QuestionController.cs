using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController(IQuestionRepository questionRepository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<QuestionDTO> GetQuestions()
    {
        return questionRepository.GetQuestions();
    }
}