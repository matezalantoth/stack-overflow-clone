using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTests;

public class SearchQuestionsByTitleTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task SearchQuestionByTitle()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var searchRes = await AHelper.SearchQuestionsByTitle(token);
        searchRes.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task SearchQuestionsByTitleRequiresAdmin()
    {
        var signupRes = await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var searchRes = await AHelper.SearchQuestionsByTitle(user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, searchRes.StatusCode);
    }
}