using System.Net.Http.Headers;
using System.Text;

namespace ElProjectGrandeTest.TestHelpers;

public class AdminControllerTestHelper(HttpClient client)
{

    public async Task<HttpResponseMessage> BanUser(string username, string token)
    {
       using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/admin/users/ban/" + username);
       requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
       return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> UnBanUser(string username, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/admin/users/unban/" + username);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> MuteUser(string username, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/admin/users/mute/" + username);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = new StringContent("{\"Time\": 30}", Encoding.UTF8, "application/json");
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> UnMuteUser(string username, string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/admin/users/unmute/" + username);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> SearchAnswersByContent(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/admin/answers/searchByContent/gibberish");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> SearchByUsername(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/admin/users/searchByUsername/username");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> SearchQuestionsByContent(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/admin/questions/searchByContent/gibberish");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> SearchQuestionsByTitle(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/admin/questions/searchByTitle/gibberish");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }
}