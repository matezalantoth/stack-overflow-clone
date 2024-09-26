using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.UserModels;

namespace ElProjectGrande.Services.AnswerServices.Repository;

public interface IAnswerRepository
{
    public IEnumerable<AnswersOfQuestionDTO> GetAllAnswersByQuestionId(Guid questionId);

    public Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question);

    public Task<Answer?> GetAnswerById(Guid id);

    public Task DeleteAnswer(Answer answer, User user);

    public Task<AnswersOfQuestionDTO> UpdateAnswer(Answer answer);

    public Task<AnswersOfQuestionDTO> AcceptAnswer(Answer answer);

    public void VoteAnswer(Answer answer, int vote);

    public IEnumerable<AdminAnswerDTO> GetAnswersByContent(string contentSubstring);

    public Task UnAcceptAnswer(Guid answerId);
}