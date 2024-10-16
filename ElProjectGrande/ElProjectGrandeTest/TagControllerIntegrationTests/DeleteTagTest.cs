using System.Net;
using System.Net.Http.Json;
using ElProjectGrande.Models.TagModels.DTOs;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest.TagControllerIntegrationTests;

public class DeleteTagTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task DeleteTag()
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

        var delRes = await TagHelper.DeleteTag(tags.First().Id, token);
        delRes.EnsureSuccessStatusCode();

        var get2Res = await TagHelper.GetAllTags();
        get2Res.EnsureSuccessStatusCode();
        var tags2 = await get2Res.Content.ReadFromJsonAsync<List<TagDTO>>();

        Assert.NotNull(tags2);
        Assert.True(tags2.Count == 0);
    }

    [Fact]
    public async Task DeleteTagRequiresAuthorization()
    {
        var signUpRes =
            await UHelper.Register("Mate", "matezalantoth", "matezalantoth@gmail.com", "admin123", "2004-09-06");
        signUpRes.EnsureSuccessStatusCode();
        var user = await signUpRes.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);

        var delRes = await TagHelper.DeleteTag(Guid.NewGuid(), user.SessionToken);
        Assert.Equal(HttpStatusCode.Forbidden, delRes.StatusCode);

    }

    [Fact]
    public async Task DeleteTagReturnsNotFoundIfTagNotFound()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);

        var delRes = await TagHelper.DeleteTag(Guid.NewGuid(), token);
        Assert.Equal(HttpStatusCode.NotFound, delRes.StatusCode);
    }
}