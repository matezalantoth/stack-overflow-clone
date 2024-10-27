using System.Net;
using System.Net.Http.Json;
using BackendServer.Models.ExceptionModels;
using BackendServer.Models.UserModels.DTOs;
using Xunit.Abstractions;

namespace BackendTest.UserControllerIntegrationTests;

public class UserControllerIntegrationTest(ITestOutputHelper outputHelper) : Tester(outputHelper)
{
    [Fact]
    public async Task RegisterUser()
    {
        var newUser = new NewUser
            { Name = "Mate", UserName = "matezal.antoth", Email = "matezal.antoth@gmail.com", Password = "admin1234" };
        var res = await UHelper.Register(newUser.Name, newUser.UserName, newUser.Email, newUser.Password, "2004-09-06");
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
        var res = await UHelper.Register(newUser.Name, newUser.UserName, newUser.Email, newUser.Password, "2004-09-06");
        var error = await res.Content.ReadFromJsonAsync<Error>();
        Assert.Multiple(() =>
        {
            Assert.NotNull(error);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            Assert.Equal("Some of your credentials are invalid", error.Message);
        });
    }

    [Fact]
    public async Task LoginAdmin()
    {
        var response = await UHelper.Login("admin@admin.com", "admin123");
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
        var response = await UHelper.Login("admin@admin.com", "admin1234");
        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Multiple(() =>
        {
            Assert.NotNull(error);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("Invalid credentials", error.Message);
        });
    }

    [Fact]
    public async Task LogoutUser()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var response = await UHelper.Logout(token);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUserWithSessionToken()
    {
        var email = "admin@admin.com";
        var loginRes = await UHelper.Login(email, "admin123");
        loginRes.EnsureSuccessStatusCode();
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var response = await UHelper.GetUserBySessionToken(token);
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
        var res = await Client.GetAsync("/getUserByUserName?username=admin");
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
        var res = await Client.GetAsync("/getUserByUserName?username=adminadmin");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task IsUserAdminWithAdmin()
    {
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var res = await UHelper.IsAdmin(token);
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
        var loginRes = await UHelper.Login("admin@admin.com", "admin123");
        var token = await loginRes.Content.ReadFromJsonAsync<string>();
        Assert.NotNull(token);
        var res = await UHelper.IsAdmin(token);
        res.EnsureSuccessStatusCode();
        var isAdmin = await res.Content.ReadFromJsonAsync<bool>();
        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.True(isAdmin);
        });
    }
}