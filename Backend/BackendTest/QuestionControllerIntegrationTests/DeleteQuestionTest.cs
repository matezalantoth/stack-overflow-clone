using System.Net;
using System.Net.Http.Json;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace BackendTest.QuestionControllerIntegrationTests;

public class DeleteQuestionTest(ITestOutputHelper outputHelper) : QuestionTester(outputHelper)
{
    [Fact]
    public async Task DeleteQuestion()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await QHelper.PostQuestion(NewQuestion, token);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var delRes = await QHelper.DeleteQuestionById(id, token);
        postRes.EnsureSuccessStatusCode();
        Assert.True(delRes.StatusCode == HttpStatusCode.NoContent);

        var getRes = await QHelper.GetQuestionById(id);
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestionIsForbiddenIfNotPostingUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await QHelper.PostQuestion(NewQuestion, token);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-04");
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var delRes = await QHelper.DeleteQuestionById(id, user.SessionToken);
        StdOut.WriteLine(await delRes.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Forbidden, delRes.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestionWorksOnEveryQuestionIfAdmin()
    {
        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-04");
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postRes = await QHelper.PostQuestion(NewQuestion, user.SessionToken);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var delRes = await QHelper.DeleteQuestionById(id, token);
        delRes.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteQuestionReturnsNotFoundOnNonexistentQuestion()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var delRes = await QHelper.DeleteQuestionById(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, delRes.StatusCode);
    }

    [Fact]
    private async Task CallingDeleteQuestionWithoutTokenReturnsUnauthorized()
    {
        var delRes = await QHelper.DeleteQuestionById(Guid.NewGuid(),
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJTZXNzaW9uVG9rZW4iLCJqdGkiOiI5Yzc1NjhiNC1mYWIwLTRjMmItOWYxZS1iMzZjZTEyNTdmMWIiLCJpYXQiOjE3Mjg5MDU1OTUsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMjcxMDM3MTYtMDQxNy00YTFiLTkzZDEtYTIxNjU4OGVlNzVjIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImFkbWluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiYWRtaW5AYWRtaW4uY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3Mjg5OTE5OTUsImlzcyI6IkdyYW5kZSIsImF1ZCI6IkdyYW5kZSJ9.sqt3w0Kr9Nb24K0dc-byBTLaAjfo-16HAvjr8wUHPOA");
        Assert.Equal(HttpStatusCode.Unauthorized, delRes.StatusCode);
    }
}