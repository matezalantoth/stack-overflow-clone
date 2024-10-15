using System.Net.Http.Headers;
using System.Text;

namespace ElProjectGrandeTest.TestHelpers;

public class UserControllerTestHelper(HttpClient client)
{
    
    public async Task<HttpResponseMessage> Login(string email, string password)
    {
        var requestBody = new StringContent(
            "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}",
            Encoding.UTF8, "application/json");
        return await client.PostAsync("/users/login", requestBody);
    }


    public async Task<HttpResponseMessage> Register(string name, string username, string email, string password, string dob)
    {
        var requestBody = new StringContent(
            "{\"name\":\"" + name + "\", " +
            "\"username\":\"" + username + "\", " +
            "\"email\":\"" + email + "\", " +
            "\"password\":\"" + password + "\", " +
            "\"dob\":\"" + dob + "\"}",
            Encoding.UTF8,
            "application/json");

        return await client.PostAsync("/users/signup", requestBody);
    }

    public async Task<HttpResponseMessage> IsAdmin(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/IsUserAdmin");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> GetUserBySessionToken(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/getbysessiontoken");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }

    public async Task<HttpResponseMessage> Logout(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/users/logout");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(requestMessage);
    }
}