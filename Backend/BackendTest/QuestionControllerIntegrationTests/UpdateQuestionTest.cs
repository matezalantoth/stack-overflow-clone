using System.Net;
using System.Net.Http.Json;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace BackendTest.QuestionControllerIntegrationTests;

public class UpdateQuestionTest(ITestOutputHelper outputHelper) : QuestionTester(outputHelper)
{
    [Fact]
    public async Task UpdateQuestion()
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

        var updRes = await QHelper.UpdateQuestionById(id, new UpdatedQuestion { Title = "", Content = "" }, token);
        updRes.EnsureSuccessStatusCode();

        var getRes = await QHelper.GetQuestionById(id);
        getRes.EnsureSuccessStatusCode();
        var newQuestion = await getRes.Content.ReadFromJsonAsync<QuestionDTO>();

        Assert.Multiple(() =>
        {
            Assert.NotNull(newQuestion);
            Assert.NotEqual(question.Title, newQuestion.Title);
            Assert.NotEqual(question.Content, newQuestion.Content);
            Assert.Equal(question.Id, newQuestion.Id);
            Assert.Equal(question.PostedAt, newQuestion.PostedAt);
            Assert.Equal(question.Username, newQuestion.Username);
            Assert.Equal(question.HasAccepted, newQuestion.HasAccepted);
        });
    }

    [Fact]
    public async Task UpdateQuestionReturnsNotFoundOnNonexistentQuestion()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var updRes =
            await QHelper.UpdateQuestionById(Guid.NewGuid(), new UpdatedQuestion { Title = "", Content = "" }, token);
        Assert.Equal(HttpStatusCode.NotFound, updRes.StatusCode);
    }

    [Fact]
    public async Task BannedUserIsForbidden()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var signupRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postRes = await QHelper.PostQuestion(NewQuestion, user.SessionToken);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var banRes = await AHelper.BanUser("matezalantoth", token);
        banRes.EnsureSuccessStatusCode();

        var updRes =
            await QHelper.UpdateQuestionById(id, new UpdatedQuestion { Title = "", Content = "" }, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, updRes.StatusCode);
    }

    [Fact]
    public async Task OnlyOriginalPosterCanUpdate()
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

        var postRes = await QHelper.PostQuestion(NewQuestion, token);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var updRes =
            await QHelper.UpdateQuestionById(id, new UpdatedQuestion { Title = "", Content = "" }, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, updRes.StatusCode);
    }

    [Fact]
    public async Task AdminCanUpdateEverything()
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

        var postRes = await QHelper.PostQuestion(NewQuestion, user.SessionToken);
        postRes.EnsureSuccessStatusCode();
        var question = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(question);
        var id = question.Id;

        var updRes = await QHelper.UpdateQuestionById(id, new UpdatedQuestion { Title = "", Content = "" }, token);
        updRes.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, updRes.StatusCode);
    }
}