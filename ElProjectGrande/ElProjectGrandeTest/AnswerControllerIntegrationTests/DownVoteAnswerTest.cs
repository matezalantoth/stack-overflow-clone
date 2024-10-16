using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.AnswerModels.DTOs;
using ElProjectGrande.Models.QuestionModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.AnswerControllerIntegrationTests;

public class DownVoteAnswerTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task DownvoteMinusesOneVoteAnswer()
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

        var downvoteRes = await AnsHelper.DownvoteAnswer(ans.Id, token);
        downvoteRes.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers);
        Assert.True(answers.Count == 1);
        Assert.True(answers[0].Votes == -1);
    }

    [Fact]
    public async Task UpvotedAnswerGetsDownvotedAnswerMinusesTwo()
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

        var upvoteRes = await AnsHelper.UpvoteAnswer(ans.Id, token);
        upvoteRes.EnsureSuccessStatusCode();
        var getRes2 = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes2.EnsureSuccessStatusCode();
        var answers2 = await getRes2.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers2);
        Assert.True(answers2.Count == 1);
        Assert.True(answers2[0].Votes == 1);

        var downvoteRes = await AnsHelper.DownvoteAnswer(ans.Id, token);
        downvoteRes.EnsureSuccessStatusCode();
        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers);
        Assert.True(answers.Count == 1);
        Assert.True(answers[0].Votes == -1);
    }

    [Fact]
    public async Task DownvotingWhenAlreadyDownvotedUnvotesAnswer()
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

        var downvoteRes = await AnsHelper.DownvoteAnswer(ans.Id, token);
        downvoteRes.EnsureSuccessStatusCode();

        var downvoteRes2 = await AnsHelper.DownvoteAnswer(ans.Id, token);
        downvoteRes2.EnsureSuccessStatusCode();

        var getRes = await AnsHelper.GetAllAnswersForQuestion(q.Id);
        getRes.EnsureSuccessStatusCode();
        var answers = await getRes.Content.ReadFromJsonAsync<List<AnswerDTO>>();
        Assert.NotNull(answers);
        Assert.True(answers.Count == 1);
        Assert.True(answers[0].Votes == 0);
    }

    [Fact]
    public async Task CanOnlyVoteOnExistingAnswers()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var upvoteRes = await AnsHelper.DownvoteAnswer(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, upvoteRes.StatusCode);
    }

    [Fact]
    public async Task VotingRequiresAuthentication()
    {
        var upvoteRes = await AnsHelper.DownvoteAnswer(Guid.NewGuid(), "");
        Assert.Equal(HttpStatusCode.Unauthorized, upvoteRes.StatusCode);
    }
}