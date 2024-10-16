using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.TagModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.TagControllerIntegrationTests;

public class CreateTagTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task CreateTag()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var postRes = await TagHelper.PostTag(new NewTag{ TagName = "name"}, token);
        postRes.EnsureSuccessStatusCode();

        var getRes = await TagHelper.GetAllTags();
        getRes.EnsureSuccessStatusCode();
        var tags = await getRes.Content.ReadFromJsonAsync<List<TagDTO>>();

        Assert.NotNull(tags);
        Assert.True(tags.Count > 0);
        Assert.Contains(tags, tag => tag.TagName == "name");
    }

    [Fact]
    public async Task CreatTagRequiresAuthorization()
    {
        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var postRes = await TagHelper.PostTag(new NewTag{ TagName = "name"}, user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, postRes.StatusCode);

    }
}