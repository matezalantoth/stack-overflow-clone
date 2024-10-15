using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace ElProjectGrandeTest;

public class UserControllerIntegrationTest
{
    private readonly ElProjectGrandeAppFactory _app = new();
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _outputHelper;

    public UserControllerIntegrationTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _client = _app.CreateClient();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

    }

    private async Task<HttpResponseMessage> Login(string email, string password)
    {
        var requestBody = new StringContent(
            "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}",
            Encoding.UTF8, "application/json");
        return await _client.PostAsync("/users/login", requestBody);
    }


    private async Task<HttpResponseMessage> Register(string name, string username, string email, string password, string dob)
    {
        var requestBody = new StringContent(
            "{\"name\":\"" + name + "\", " +
            "\"username\":\"" + username + "\", " +
            "\"email\":\"" + email + "\", " +
            "\"password\":\"" + password + "\", " +
            "\"dob\":\"" + dob + "\"}",
            Encoding.UTF8,
            "application/json");

        return await _client.PostAsync("/users/signup", requestBody);
    }

    private async Task<HttpResponseMessage> IsAdmin(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/IsUserAdmin");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(requestMessage);
    }

    private async Task<HttpResponseMessage> GetUserBySessionToken(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/getbysessiontoken");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(requestMessage);
    }

    private async Task<HttpResponseMessage> Logout(string token)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/users/logout");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(requestMessage);
    }

    [Fact]
    public async Task RegisterUser()
    {
        var newUser = new NewUser
            { Name = "Mate", UserName = "matezal.antoth", Email = "matezal.antoth@gmail.com", Password = "admin1234" };
        var res = await Register(newUser.Name, newUser.UserName, newUser.Email, newUser.Password, "2004-09-06");
        var user = await res.Content.ReadFromJsonAsync<UserDTO>();
        res.EnsureSuccessStatusCode();

        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.NotNull(user);
            Assert.Equal(newUser.Name, user.Name);
            Assert.Equal(newUser.Email, user.Email);
            Assert.Equal(newUser.UserName, user.UserName);
            Assert.NotNull(user.SessionToken);
        });
    }

    [Fact]
    public async Task RegisterFailsIfUserExists()
    {
        var newUser = new NewUser
            { Name = "admin", UserName = "admin", Email = "admin@admin.com", Password = "admin1234" };
        var res = await Register(newUser.Name, newUser.UserName, newUser.Email, newUser.Password, "2004-09-06");
        var resMessage = await res.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            Assert.Equal("Some of your credentials are invalid", resMessage);
        });
    }

    [Fact]
    public async Task LoginAdmin()
    {
        var response = await Login("admin@admin.com", "admin123");
        response.EnsureSuccessStatusCode();
        var resMessage = await response.Content.ReadAsStringAsync();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(resMessage);
        });

    }

    [Fact]
    public async Task LoginAdminWithBadCredentialsReturnsBadRequest()
    {
        var response = await Login("admin@admin.com", "admin1234");
        var resMessage = await response.Content.ReadFromJsonAsync<string>();
       Assert.Multiple(() =>
       {
           Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
           Assert.Equal("Invalid credentials", resMessage);
       });
    }

    [Fact]
    public async Task LogoutUser()
    {
        var loginRes = await Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var response = await Logout(token);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUserWithSessionToken()
    {
        var email = "admin@admin.com";
        var loginRes = await Login(email, "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var response = await GetUserBySessionToken(token);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(email, user.Email);
        });
    }

    //May need to change dto, if changes are made to the controller
    [Fact]
    public async Task GetUserWithUsername()
    {
        var res= await _client.GetAsync("/getUserByUserName?username=admin");
        res.EnsureSuccessStatusCode();
        var user = await res.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Equal("admin", user.UserName);
        });
    }


    [Fact]
    public async Task GetUserWithUsernameCallingNonexistentUserReturnsNotFound()
    {
        var res= await _client.GetAsync("/getUserByUserName?username=adminadmin");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task IsUserAdminWithAdmin()
    {
        var loginRes = await Login("admin@admin.com", "admin123");
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var res = await IsAdmin(token);
        res.EnsureSuccessStatusCode();
        var isAdmin = await res.Content.ReadFromJsonAsync<bool>();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.True(isAdmin);
        });
    }

    [Fact]
    public async Task IsUserAdminWithNormalUser()
    {
        var loginRes = await Login("admin@admin.com", "admin123");
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var res = await IsAdmin(token);
        res.EnsureSuccessStatusCode();
        var isAdmin = await res.Content.ReadFromJsonAsync<bool>();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.True(isAdmin);
        });
    }
}