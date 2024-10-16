using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTests;

public class UnBanUserTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task UnBan()
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

        var banRes = await AHelper.BanUser("matezalantoth", token);
        banRes.EnsureSuccessStatusCode();

        var getRes = await UHelper.GetUserByUsername("matezalantoth");
        getRes.EnsureSuccessStatusCode();
        var bannedUser = await getRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(bannedUser);
        Assert.True(bannedUser.Banned);

        var unBanRes = await AHelper.UnBanUser("matezalantoth", token);
        unBanRes.EnsureSuccessStatusCode();

        var unbanGetRes = await UHelper.GetUserByUsername("matezalantoth");
        unbanGetRes.EnsureSuccessStatusCode();
        var unbannedUser = await unBanRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(unbannedUser);
        Assert.False(unbannedUser.Banned);
    }

    [Fact]
    public async Task UnBanningNonexistentUserReturnsNotFound()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var unBanRes = await AHelper.UnBanUser("fake.user", token);
        Assert.Equal(HttpStatusCode.NotFound, unBanRes.StatusCode);
    }

    [Fact]
    public async Task OnlyAdminCanUnBanUser()
    {
        var signUpRes = await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var unbanRes = await AHelper.UnBanUser("fake.user", user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, unbanRes.StatusCode);
    }

    [Fact]
    public async Task UnBanUserRequiresAuthorization()
    {
        var unBanRes = await AHelper.UnBanUser("matezalantoth", "");
        Assert.Equal(HttpStatusCode.Unauthorized, unBanRes.StatusCode);
    }
}