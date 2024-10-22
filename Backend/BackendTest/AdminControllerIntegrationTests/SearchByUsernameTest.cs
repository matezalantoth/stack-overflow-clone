using System.Net;
using System.Net.Http.Json;
using BackendServer.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace BackendTest.AdminControllerIntegrationTests;

public class SearchByUsernameTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task SearchByUsername()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var searchRes = await AHelper.SearchByUsername(token);
        searchRes.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task SearchByUsernameRequiresAdmin()
    {
        var signupRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var searchRes = await AHelper.SearchByUsername(user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, searchRes.StatusCode);
    }
}