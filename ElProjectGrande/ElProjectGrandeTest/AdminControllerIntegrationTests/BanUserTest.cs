using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTests;

public class BanUserTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task BanUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var banRes = await AHelper.BanUser(user.UserName, token);
        banRes.EnsureSuccessStatusCode();

        var getRes = await UHelper.GetUserByUsername(user.UserName);
        getRes.EnsureSuccessStatusCode();
        var bannedUser = await getRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(bannedUser);
        Assert.True(bannedUser.Banned);
    }

    [Fact]
    public async Task BanningNonexistentUserReturnsNotFound()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var banRes = await AHelper.BanUser("fake.username", token);
        var message = await banRes.Content.ReadFromJsonAsync<string>();
        Assert.Equal(HttpStatusCode.NotFound, banRes.StatusCode);
        Assert.Equal("This user could not be found", message);
    }

    [Fact]
    public async Task OnlyAdminsCanBan()
    {
        var signupRes = await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var banRes = await AHelper.BanUser("admin", user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, banRes.StatusCode);
    }

    [Fact]
    public async Task BanRequiresTokenAndReturnsUnauthorizedOtherwise()
    {
        var banRes = await AHelper.BanUser("fake.username", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJTZXNzaW9uVG9rZW4iLCJqdGkiOiI5Yzc1NjhiNC1mYWIwLTRjMmItOWYxZS1iMzZjZTEyNTdmMWIiLCJpYXQiOjE3Mjg5MDU1OTUsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMjcxMDM3MTYtMDQxNy00YTFiLTkzZDEtYTIxNjU4OGVlNzVjIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImFkbWluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AYWRtaW4uY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3Mjg5OTE5OTUsImlzcyI6IkdyYW5kZSIsImF1ZCI6IkdyYW5kZSJ9.sqt3w0Kr9Nb24K0dc-byBTLaAjfo-16HAvjr8wUHPOA");
        Assert.True(banRes.StatusCode == HttpStatusCode.Unauthorized);
    }
}