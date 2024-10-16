using ElProjectGrande.Models;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.QuestionServices.Factory;

public class QuestionFactory : IQuestionFactory
{

    public Question CreateNewUpdatedQuestionFromUpdatesAndOriginal(UpdatedQuestion updatedQuestion, Question question)
    {
        question.Title = updatedQuestion.Title;
        question.Content = updatedQuestion.Content;
        return question;
    }
}