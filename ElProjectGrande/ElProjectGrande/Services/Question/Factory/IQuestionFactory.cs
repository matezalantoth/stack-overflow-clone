using ElProjectGrande.Models;

namespace ElProjectGrande.Services;

public interface IQuestionFactory
{
    public Question CreateQuestion(NewQuestion newQuestion, User user);

    public Question CreateNewUpdatedQuestionFromUpdatesAndOriginal(UpdatedQuestion updatedQuestion,
        Question question);
}