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
}