using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTest;

public class SearchAnswersByContentTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task SearchAnswersByContent()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var searchRes = await AHelper.SearchAnswersByContent(token);
        searchRes.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task SearchAnswersByContentRequiresAdmin()
    {
        var signupRes = await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var searchRes = await AHelper.SearchAnswersByContent(user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, searchRes.StatusCode);
    }
}