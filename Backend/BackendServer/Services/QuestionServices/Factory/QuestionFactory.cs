
using BackendServer.Models.QuestionModels;
using BackendServer.Models.QuestionModels.DTOs;

namespace BackendServer.Services.QuestionServices.Factory;

public class QuestionFactory : IQuestionFactory
{

    public Question CreateNewUpdatedQuestionFromUpdatesAndOriginal(UpdatedQuestion updatedQuestion, Question question)
    {
        question.Title = updatedQuestion.Title;
        question.Content = updatedQuestion.Content;
        return question;
    }
}