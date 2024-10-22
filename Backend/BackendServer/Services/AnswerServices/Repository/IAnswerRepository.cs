

using BackendServer.Models.AnswerModels;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels;
using BackendServer.Models.UserModels;

namespace BackendServer.Services.AnswerServices.Repository;

public interface IAnswerRepository
{
    public IEnumerable<AnswerDTO> GetAllAnswersByQuestionId(Guid questionId);

    public Task<AnswerDTO> CreateAnswer(Answer answer, User user, Question question);

    public Task<Answer?> GetAnswerById(Guid id);

    public Task DeleteAnswer(Answer answer, User user);

    public Task<AnswerDTO> UpdateAnswer(Answer answer);

    public Task<AnswerDTO> AcceptAnswer(Answer answer);

    public void VoteAnswer(Answer answer, int vote);

    public IEnumerable<AdminAnswerDTO> GetAnswersByContent(string contentSubstring);

    public Task<Answer> UnAcceptAnswer(Guid answerId);

    public Task<Question> GetQuestionOfAnswerByAnswerId(Guid answerId);
}