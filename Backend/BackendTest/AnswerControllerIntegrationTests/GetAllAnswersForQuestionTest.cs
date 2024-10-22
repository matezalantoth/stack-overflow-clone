using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace BackendTest.AnswerControllerIntegrationTests;

public class GetAllAnswersForQuestionTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task GetFailsIfQuestionDoesNotExist()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var getRes = await AnsHelper.GetAllAnswersForQuestion(Guid.NewGuid());
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }
}