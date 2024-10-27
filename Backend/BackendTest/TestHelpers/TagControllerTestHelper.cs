using System.Net.Http.Headers;
using System.Text;
using BackendServer.Models.TagModels.DTOs;
using Newtonsoft.Json;

namespace BackendTest.TestHelpers;

public class TagControllerTestHelper(HttpClient client)
{
    public async Task<HttpResponseMessage> GetAllTags()
    {
        return await client.GetAsync("/tags");
    }

    public async Task<HttpResponseMessage> PostTag(NewTag newTag, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/tags");
        request.Content = new StringContent(JsonConvert.SerializeObject(newTag), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteTag(Guid id, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, "/tags/" + id);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }
}