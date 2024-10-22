using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackendServer.Models.QuestionModels.DTOs;

namespace BackendTest.TestHelpers;

public class QuestionControllerTestHelper(HttpClient client)
{
    public async Task<HttpResponseMessage> PostQuestion(NewQuestion question, string token)
    {
        var reqBody = JsonSerializer.Serialize(question);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/questions");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = new StringContent(reqBody, Encoding.UTF8, "application/json");
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> GetQuestionById(Guid id)
    {
        return await client.GetAsync($"/questions/{id}");
    }

    public async Task<HttpResponseMessage> DeleteQuestionById(Guid id, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/questions/{id}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> UpdateQuestionById(Guid id, UpdatedQuestion question, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/questions/{id}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content =
            new StringContent(JsonSerializer.Serialize(question), Encoding.UTF8, "application/json");
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> GetQuestions(int startIndex)
    {
        return await client.GetAsync("/questions?startIndex=" + startIndex);
    }

    public async Task<HttpResponseMessage> GetTrendingQuestions()
    {
        return await client.GetAsync("questions/trending");
    }
}