using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AdminControllerIntegrationTests;

public class UnAcceptAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task UnAcceptAnswer()
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

        var acceptRes = await AnsHelper.AcceptAnswer(ans.Id, token);
        acceptRes.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers);
        Assert.Contains(answers, a => a.Accepted);

        var unAcceptRes = await AHelper.UnAcceptAnswer(ans.Id, token);
        unAcceptRes.EnsureSuccessStatusCode();

        var getRes2 = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers2 = await getRes2.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers2);
        Assert.True(answers2.Count(a => a.Accepted) == 0);

    }

    [Fact]
    public async Task UnAcceptAnswerRequiresAuthorization()
    {
        var signupRes = await UHelper.Register("mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signupRes.EnsureSuccessStatusCode();
        var user = await signupRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var unAcceptRes = await AHelper.UnAcceptAnswer(Guid.NewGuid(), user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, unAcceptRes.StatusCode);
    }

    [Fact]
    public async Task CanOnlyUnAcceptExistingAnswers()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var unAcceptRes = await AHelper.UnAcceptAnswer(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, unAcceptRes.StatusCode);
    }
}