using System.Net.Http.Headers;
using System.Text;
using ElProjectGrande.Models.AnswerModels.DTOs;
using Newtonsoft.Json;

namespace ElProjectGrandeTest.TestHelpers;

public class AnswerControllerTestHelper(HttpClient client)
{
    public async Task<HttpResponseMessage> GetAllAnswersForQuestion(Guid questionId)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/answers?questionId=" + questionId);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> PostNewAnswerToQuestion(Guid questionId, NewAnswer newAnswer, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/answers?questionId=" + questionId);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(newAnswer), Encoding.UTF8, "application/json");
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> DeleteAnswer(Guid answerId, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/answers/" + answerId);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> UpdateAnswer(Guid answerId, string newContent, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/answers/" + answerId);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = new StringContent("\""+ newContent + "\"", Encoding.UTF8, "application/json");
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> AcceptAnswer(Guid answerId, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/accept/" + answerId);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> UpvoteAnswer(Guid answerId, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/answers/{answerId}/upvote");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> DownvoteAnswer(Guid answerId, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/answers/{answerId}/downvote");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> GetQuestionByAnswer(Guid answerId, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/answers/question/" + answerId);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

}