using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.QuestionModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.QuestionControllerIntegrationTests;

public class GetQuestionByIdTest(ITestOutputHelper outputHelper) : QuestionTester(outputHelper)
{

    [Fact]
    public async Task GetQuestionById()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRequest = await QHelper.PostQuestion(NewQuestion, token);
        postRequest.EnsureSuccessStatusCode();
        var newQuestion = await postRequest.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.NotNull(newQuestion);
        var id = newQuestion.Id;

        var questionRes = await QHelper.GetQuestionById(id);
        questionRes.EnsureSuccessStatusCode();
        var question = await questionRes.Content.ReadFromJsonAsync<QuestionDTO>();
        Assert.Multiple(() =>
        {
            Assert.NotNull(question);
            Assert.Equal(newQuestion.Title, question.Title);
            Assert.Equal(newQuestion.Content, question.Content);
            Assert.Equal(newQuestion.PostedAt, question.PostedAt);
            Assert.Equal(HttpStatusCode.OK, questionRes.StatusCode);
        });
    }

    [Fact]
    public async Task ReturnsNotFoundIfQuestionDoesNotExist()
    {
        var res = await QHelper.GetQuestionById(Guid.NewGuid());
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}