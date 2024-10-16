using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AnswerControllerIntegrationTests;

public class GetQuestionOfAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task GetQuestionOfAnswer()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await QHelper.PostQuestion(new NewQuestion{ Title="test", Content="test", PostedAt = DateTime.Now, Tags = []}, token);
        postRes.EnsureSuccessStatusCode();
        var q = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id, new NewAnswer { Content="tester", PostedAt = DateTime.Now}, token);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var getRes = await AnsHelper.GetQuestionByAnswer(ans.Id, token);
        getRes.EnsureSuccessStatusCode();
        var questionId = await getRes.Content.ReadFromJsonAsync<Guid>();
        Assert.Equal(q.Id, questionId);
    }

    [Fact]
    public async Task GetQuestionOfAnswerRequiresAuthorization()
    {
        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postRes = await QHelper.PostQuestion(new NewQuestion{ Title="test", Content="test", PostedAt = DateTime.Now, Tags = []}, user.SessionToken);
        postRes.EnsureSuccessStatusCode();
        var q = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id, new NewAnswer { Content="tester", PostedAt = DateTime.Now}, user.SessionToken);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var getRes = await AnsHelper.GetQuestionByAnswer(ans.Id, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, getRes.StatusCode);
    }

    [Fact]
    public async Task GetQuestionOfAnswerReturnsNotFoundIfAnswerNotFound()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await QHelper.PostQuestion(new NewQuestion{ Title="test", Content="test", PostedAt = DateTime.Now, Tags = []}, token);
        postRes.EnsureSuccessStatusCode();
        var q = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var getRes = await AnsHelper.GetQuestionByAnswer(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }
}