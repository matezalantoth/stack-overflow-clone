using System.Text;
using Xunit.Abstractions;

namespace ElProjectGrandeTest;

public class UserControllerIntegrationTest
{
    private readonly ElProjectGrandeAppFactory _app = new();
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _outputHelper;

    public UserControllerIntegrationTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _client = _app.CreateClient();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

    }

    [Fact]
    public async Task LoginAdmin()
    {
        var requestBody = new StringContent(
            "{\"email\":\"admin@admin.com\", \"password\":\"admin123\"}",
            Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/users/login", requestBody);
        _outputHelper.WriteLine(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();

    }
}