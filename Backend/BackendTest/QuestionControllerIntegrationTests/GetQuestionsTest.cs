using System.Net.Http.Json;
using Xunit.Abstractions;

namespace BackendTest.QuestionControllerIntegrationTests;

public class GetQuestionsTest(ITestOutputHelper outputHelper) : QuestionTester(outputHelper)
{
    [Fact]
    public async Task GetQuestions()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var searchRes = await QHelper.GetQuestions(0);
        searchRes.EnsureSuccessStatusCode();
    }
}