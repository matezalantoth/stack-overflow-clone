using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AnswerControllerIntegrationTests;

public class AcceptAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task AcceptAnswer()
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
    }

    [Fact]
    public async Task CanOnlyAcceptAnAnswerIfQuestionHasNoAcceptedAnswers()
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

        var postAns2Res = await AnsHelper.PostNewAnswerToQuestion(q.Id, new NewAnswer { Content="testing", PostedAt = DateTime.Now}, token);
        postAns2Res.EnsureSuccessStatusCode();
        var ans2 = await postAns2Res.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans2);

        var accept2Res = await AnsHelper.AcceptAnswer(ans2.Id, token);
        Assert.Equal(HttpStatusCode.BadRequest, accept2Res.StatusCode);
    }

    [Fact]
    public async Task AdminCanAcceptAnyAnswer()
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

        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var acceptRes = await AnsHelper.AcceptAnswer(ans.Id, token);
        acceptRes.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers);
        Assert.Contains(answers, a => a.Accepted);
    }

    [Fact]
    public async Task AcceptAnswerRequiresAuthorization()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await QHelper.PostQuestion(new NewQuestion{ Title="test", Content="test", PostedAt = DateTime.Now, Tags = []}, token);
        postRes.EnsureSuccessStatusCode();
        var q = await postRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(q);

        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(q.Id, new NewAnswer { Content="tester", PostedAt = DateTime.Now}, user.SessionToken);
        postAnsRes.EnsureSuccessStatusCode();
        var ans = await postAnsRes.Content.ReadFromJsonAsync<AnswerDTO>();
        Assert.NotNull(ans);

        var acceptRes = await AnsHelper.AcceptAnswer(ans.Id, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, acceptRes.StatusCode);
    }

    [Fact]
    public async Task AcceptAnswerOnlyWorksOnExistingAnswers()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var acceptRes = await AnsHelper.AcceptAnswer(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, acceptRes.StatusCode);
    }
}