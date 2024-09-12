using ElProjectGrande.Models;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.QuestionServices.Factory;

public interface IQuestionFactory
{
    public Question CreateQuestion(NewQuestion newQuestion, User user);

    public Question CreateNewUpdatedQuestionFromUpdatesAndOriginal(UpdatedQuestion updatedQuestion,
        Question question);
}