using ElProjectGrande.Exceptions;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.AuthenticationServices.TokenService;
using ElProjectGrande.Services.UserServices.Factory;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserRepository userRepository, IUserFactory userFactory, ITokenService tokenService)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<Guid>> CreateUserAndLogin([FromBody] NewUser newUser)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await userRepository.AreCredentialsTaken(newUser.Email, newUser.UserName))
            return BadRequest("Some of your credentials are invalid");

        var user = userFactory.CreateUser(newUser);
        await userRepository.CreateUser(user, newUser.Password, "User");
        user.SessionToken = await userRepository.LoginUser(newUser.Email, newUser.Password);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginCredentials loginCredentials)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var token = await userRepository.LoginUser(loginCredentials.Email, loginCredentials.Password);
        return Content($"\"{token}\"", "application/json");
    }


    [HttpPost("logout")]
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LogoutUser([FromHeader(Name = "Authorization")] string sessionToken)
    {
        sessionToken = tokenService.ValidateAndGetSessionToken(sessionToken);
        if (!userRepository.IsUserLoggedIn(sessionToken)) throw new NotFoundException("This user could not be found");
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
    [Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<UserDTO>> GetUserByUserName(string userName)
    {
        var user = await userRepository.GetUserByUserName(userName);
        if (user == null) throw new NotFoundException($"User of session token: {userName} could not be found");

        return Ok(user.ToDTO());
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
}