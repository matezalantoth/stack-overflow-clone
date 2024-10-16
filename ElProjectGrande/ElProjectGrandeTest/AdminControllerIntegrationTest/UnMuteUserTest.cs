using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTest;

public class UnMuteUserTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task UnMuteUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var signUpRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();

        var muteRes = await AHelper.MuteUser("matezalantoth", token);
        muteRes.EnsureSuccessStatusCode();

        var getRes = await UHelper.GetUserByUsername("matezalantoth");
        getRes.EnsureSuccessStatusCode();
        var user = await getRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);
        Assert.True(user.Muted);

        var unMuteRes = await AHelper.UnMuteUser("matezalantoth", token);
        unMuteRes.EnsureSuccessStatusCode();

        var unMutedGetRes = await UHelper.GetUserByUsername("matezalantoth");
        unMutedGetRes.EnsureSuccessStatusCode();
        var unMuteUser = await unMutedGetRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(unMuteUser);
        Assert.False(unMuteUser.Muted);
    }

    [Fact]
    public async Task CanOnlyUnMuteExistingUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var unMuteRes = await AHelper.UnMuteUser("fake.user", token);
        Assert.Equal(HttpStatusCode.NotFound, unMuteRes.StatusCode);
    }

    [Fact]
    public async Task OnlyAdminCanUnmuteUser()
    {
        var signUpRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var unMuteRes = await AHelper.UnMuteUser("admin", user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, unMuteRes.StatusCode);
    }

    [Fact]
    public async Task UnMuteRequiresAuthorization()
    {
        var unMuteRes = await AHelper.UnMuteUser("fake.user", "");
        Assert.Equal(HttpStatusCode.Unauthorized, unMuteRes.StatusCode);
    }
}