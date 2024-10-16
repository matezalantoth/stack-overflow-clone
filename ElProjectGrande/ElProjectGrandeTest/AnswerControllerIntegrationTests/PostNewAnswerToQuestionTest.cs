using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AnswerControllerIntegrationTests;

public class PostNewAnswerToQuestionTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task PostNewAnswer()
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
        Assert.Equal(HttpStatusCode.OK, postAnsRes.StatusCode);

        var getAnsOfQ = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getAnsOfQ.EnsureSuccessStatusCode();
        var listOfAns = await getAnsOfQ.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(listOfAns);
        Assert.Contains(listOfAns, ans => ans.Content == "tester");
    }

    [Fact]
    public async Task CanOnlyPostToExistingQuestions()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(Guid.NewGuid(), new NewAnswer { Content="tester", PostedAt = DateTime.Now}, token);
        Assert.Equal(HttpStatusCode.NotFound, postAnsRes.StatusCode);
    }

    [Fact]
    public async Task PostNewAnswerRequiresAuthentication()
    {
        var postAnsRes = await AnsHelper.PostNewAnswerToQuestion(Guid.NewGuid(), new NewAnswer { Content="tester", PostedAt = DateTime.Now}, "");
        Assert.Equal(HttpStatusCode.Unauthorized, postAnsRes.StatusCode);

    }
}