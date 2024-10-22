using System.Net;
using System.Net.Http.Json;
using BackendServer.Models.AnswerModels.DTOs;
using BackendServer.Models.QuestionModels.DTOs;
using BackendServer.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace BackendTest.AnswerControllerIntegrationTests;

public class DeleteAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task DeleteAnswer()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postQRes = await QHelper.PostQuestion(
            new NewQuestion { Title = "title", Content = "content", PostedAt = DateTime.Now, Tags = [] }, token);
        postQRes.EnsureSuccessStatusCode();
        var q = await postQRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id,
            new NewAnswer { Content = "content", PostedAt = DateTime.Now }, token);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var delRes = await AnsHelper.DeleteAnswer(ans.Id, token);
        delRes.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var listOfAns = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(listOfAns);

        Assert.True(listOfAns.Count == 0);
    }

    [Fact]
    public async Task DeleteAnswerRequiresAuthorization()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postQRes = await QHelper.PostQuestion(
            new NewQuestion { Title = "title", Content = "content", PostedAt = DateTime.Now, Tags = [] }, token);
        postQRes.EnsureSuccessStatusCode();
        var q = await postQRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id,
            new NewAnswer { Content = "content", PostedAt = DateTime.Now }, token);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var delRes = await AnsHelper.DeleteAnswer(ans.Id, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, delRes.StatusCode);
    }

    [Fact]
    public async Task AdminCanDeleteAnyAnswer()
    {
        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postQRes = await QHelper.PostQuestion(
            new NewQuestion { Title = "title", Content = "content", PostedAt = DateTime.Now, Tags = [] },
            user.SessionToken);
        postQRes.EnsureSuccessStatusCode();
        var q = await postQRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id,
            new NewAnswer { Content = "content", PostedAt = DateTime.Now }, user.SessionToken);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var delRes = await AnsHelper.DeleteAnswer(ans.Id, token);
        delRes.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var listOfAns = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(listOfAns);

        Assert.True(listOfAns.Count == 0);
    }

    [Fact]
    public async Task CanOnlyDeleteExistingAnswers()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var delRes = await AnsHelper.DeleteAnswer(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, delRes.StatusCode);
    }
}