using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.QuestionControllerIntegrationTests;

public class PostQuestionTest(ITestOutputHelper outputHelper) : QuestionTester(outputHelper)
{
    [Fact]
    public async Task PostQuestion()
    {
        var res = await UHelper.Login("admin@admin.com", "admin123");
        res.EnsureSuccessStatusCode();
        var token = await res.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var oldUserRes = await UHelper.GetUserBySessionToken(token);
        oldUserRes.EnsureSuccessStatusCode();
        var karma = (await oldUserRes.Content.ReadFromJsonAsync<UserDTO>())?.Karma;
        Assert.NotNull(karma);

        var postRes = await QHelper.PostQuestion(NewQuestion, token);
        postRes.EnsureSuccessStatusCode();

        var newerUserRes = await UHelper.GetUserBySessionToken(token);
        var newKarma = (await newerUserRes.Content.ReadFromJsonAsync<UserDTO>())?.Karma;

        Assert.NotNull(newKarma);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, postRes.StatusCode);
            Assert.Equal(karma + 5, newKarma);
        });
    }

    [Fact]
    public async Task LoggedOutUserIsUnauthorized()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var logoutRes = await UHelper.Logout(token);
        logoutRes.EnsureSuccessStatusCode();
        var postRes = await QHelper.PostQuestion(NewQuestion, token);
        Assert.Equal(HttpStatusCode.Unauthorized, postRes.StatusCode);
    }

    [Fact]
    public async Task BannedUserIsForbidden()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var registerRes =
            await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        registerRes.EnsureSuccessStatusCode();
        await AHelper.BanUser("matezalantoth", token);
        var user= await (await Client.GetAsync("/getUserByUserName?username=matezalantoth")).Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);
        Assert.True(user.Banned);
        var bannedUserLoginRes = await UHelper.Login("matezalantoth@gmail.com", "admin123");
        bannedUserLoginRes.EnsureSuccessStatusCode();
        var userToken = await bannedUserLoginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(userToken);
        var postRes = await QHelper.PostQuestion(NewQuestion, userToken);
        Assert.Equal(HttpStatusCode.Forbidden, postRes.StatusCode);
    }

    [Fact]
    public async Task CallingWithoutTokenReturnsUnauthorized()
    {
        var postRes = await QHelper.PostQuestion(NewQuestion, string.Empty);
        Assert.Equal(HttpStatusCode.Unauthorized, postRes.StatusCode);
    }
}