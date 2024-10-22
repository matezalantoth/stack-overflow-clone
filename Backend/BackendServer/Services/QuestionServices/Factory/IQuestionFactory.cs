

using BackendServer.Models.QuestionModels;
using BackendServer.Models.QuestionModels.DTOs;

namespace BackendServer.Services.QuestionServices.Factory;

public interface IQuestionFactory
{


    public Question CreateNewUpdatedQuestionFromUpdatesAndOriginal(UpdatedQuestion updatedQuestion,
        Question question);
}