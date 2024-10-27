using BackendServer.Exceptions;
using BackendServer.Extensions;
using BackendServer.Models;
using BackendServer.Models.UserModels;
using BackendServer.Models.UserModels.DTOs;
using BackendServer.Services.AuthenticationServices.TokenService;
using BackendServer.Services.UserServices.Factory;
using BackendServer.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServer.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserRepository userRepository, IUserFactory userFactory, ITokenService tokenService)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<UserDTO>> CreateUserAndLogin([FromBody] NewUser newUser)
    {
        if (await userRepository.AreCredentialsTaken(newUser.Email, newUser.UserName))
            throw new BadRequestException("Some of your credentials are invalid");
        var user = userFactory.CreateUser(newUser);
        await userRepository.CreateUser(user, newUser.Password, "User");
        user.SessionToken = await userRepository.LoginUser(newUser.Email, newUser.Password);
        return Ok(user.ToDTO());
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginCredentials loginCredentials)
    {
        var token = await userRepository.LoginUser(loginCredentials.Email, loginCredentials.Password);
        return Content($"\"{token}\"", "application/json");
    }


    [HttpPost("logout")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LogoutUser([FromHeader(Name = "Authorization")] string sessionToken)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        await userRepository.LogoutUser(sessionToken);
        return Ok();
    }

    [HttpGet("GetBySessionToken")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<UserDTO>> GetUser([FromHeader(Name = "Authorization")] string sessionToken)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionToken(sessionToken);
        if (user == null) throw new NotFoundException($"User of session token: {sessionToken} could not be found");
        return Ok(user.ToDTO());
    }

    [HttpGet("/getUserByUserName")]
    public async Task<ActionResult<UserDTO>> GetUserByUserName(string userName)
    {
        var user = await userRepository.GetUserByUserName(userName);

        if (user == null) throw new NotFoundException($"User of session token: {userName} could not be found");

        return Ok(user.ToPublicDTO());
    }

    [HttpGet("IsUserAdmin")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<bool>> IsUserAdmin([FromHeader(Name = "Authorization")] string sessionToken)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionToken(sessionToken) ??
                   throw new NotFoundException("This user could not be found");

        return Ok(userRepository.IsUserAdmin(user));
    }

    [Authorize(Roles = "User")]
    [HttpPatch("update-profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest updateProfileRequest,
        [FromHeader(Name = "Authorization")] string sessionToken)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);

        var user = await userRepository.GetUserBySessionToken(sessionToken) ??
                   throw new NotFoundException("User could not be found");


        if (!string.IsNullOrEmpty(updateProfileRequest.Email)) user.Email = updateProfileRequest.Email;


        await userRepository.UpdateUser(user,
            string.IsNullOrEmpty(updateProfileRequest.Password) ? null : updateProfileRequest.Password);
        return Ok("Profile updated successfully");
    }

    [HttpPost("VerifyUser")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<VerifyUserDTO>> VerifyUser([FromHeader(Name = "Authorization")] string sessionToken,
        [FromBody] string password)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        var user = await userRepository.GetUserBySessionToken(sessionToken) ?? throw new NotFoundException();

        return Ok(new VerifyUserDTO
        {
            Email = user.Email ?? throw new Exception(), Verified = await userRepository.VerifyUser(user, password)
        });
    }

    [HttpGet("/ping")]
    [Authorize(Roles = "Admin, User")]
    public ActionResult Ping()
    {
        return Ok();
    }
}