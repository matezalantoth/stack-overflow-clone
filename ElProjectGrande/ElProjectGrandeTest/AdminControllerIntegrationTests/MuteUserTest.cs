using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTests;

public class MuteUserTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task MuteUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var signUpRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var muteRes = await AHelper.MuteUser("matezalantoth", token);
        muteRes.EnsureSuccessStatusCode();

        var getRes = await UHelper.GetUserByUsername("matezalantoth");
        getRes.EnsureSuccessStatusCode();
        var mutedUser = await getRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(mutedUser);
        Assert.True(mutedUser.Muted);
        Assert.True(DateTime.Now < mutedUser.MutedUntil);
    }

    [Fact]
    public async Task ReMutingAddsToTimeUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var signUpRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var muteRes = await AHelper.MuteUser("matezalantoth", token);
        muteRes.EnsureSuccessStatusCode();

        var getRes = await UHelper.GetUserByUsername("matezalantoth");
        getRes.EnsureSuccessStatusCode();
        var mutedUser = await getRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(mutedUser);

        var secondMuteRes = await AHelper.MuteUser("matezalantoth", token);
        secondMuteRes.EnsureSuccessStatusCode();

        var getRes2 = await UHelper.GetUserByUsername("matezalantoth");
        getRes2.EnsureSuccessStatusCode();
        var mutedUser2 = await getRes2.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(mutedUser2);
        Assert.True(mutedUser2.Muted);
        Assert.True(mutedUser2.MutedUntil > mutedUser.MutedUntil);
    }

    [Fact]
    public async Task OnlyAdminCanMuteUser()
    {
        var signUpRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var muteRes = await AHelper.MuteUser("admin", user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, muteRes.StatusCode);
    }

    [Fact]
    public async Task CantMuteNonexistentUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var muteRes = await AHelper.MuteUser("fake.username", token);
        Assert.Equal(HttpStatusCode.NotFound, muteRes.StatusCode);
    }

    [Fact]
    public async Task MutingRequiresAuthorization()
    {
        var muteRes = await AHelper.MuteUser("fake.username", "");
        Assert.Equal(HttpStatusCode.Unauthorized, muteRes.StatusCode);
    }
}