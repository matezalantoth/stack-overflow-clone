using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.ExceptionModels;
using ElProjectGrande.Models.QuestionModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AnswerControllerIntegrationTests;

public class UpdateAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task UpdateAnswer()
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

        var updRes = await AnsHelper.UpdateAnswer(ans.Id, "testing", token);
        updRes.EnsureSuccessStatusCode();

        var getAnsOfQ = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getAnsOfQ.EnsureSuccessStatusCode();
        var listOfAns = await getAnsOfQ.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(listOfAns);
        Assert.Contains(listOfAns, a => a.Content == "testing");
    }

    [Fact]
    public async Task UpdateAnswerRequiresAuthorization()
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

        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var updRes = await AnsHelper.UpdateAnswer(ans.Id, "testing", user.SessionToken);
        var error = await updRes.Content.ReadFromJsonAsync<Error>();
        Assert.NotNull(error);
        Assert.Equal(HttpStatusCode.Forbidden, updRes.StatusCode);
        Assert.Equal("You did not post this answer, you don't have permission to edit it",
            error.Message);
    }

    [Fact]
    public async Task AdminCanUpdateAnyAnswer()
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

        var updRes = await AnsHelper.UpdateAnswer(ans.Id, "testing", token);
        updRes.EnsureSuccessStatusCode();

        var getAnsOfQ = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getAnsOfQ.EnsureSuccessStatusCode();
        var listOfAns = await getAnsOfQ.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(listOfAns);
        Assert.Contains(listOfAns, a => a.Content == "testing");
    }

    [Fact]
    public async Task CanOnlyUpdateExistingAnswers()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var updRes = await AnsHelper.UpdateAnswer(Guid.NewGuid(), "testing", token);
        Assert.Equal(HttpStatusCode.NotFound, updRes.StatusCode);
    }
}