using System.Text.Json;
using System.Text.Json.Serialization;
using ElProjectGrande.Extensions;
using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels.DTOs;
using ElProjectGrande.Services.UserServices.Factory;
using ElProjectGrande.Services.UserServices.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserRepository userRepository, IUserFactory userFactory)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<Guid>> CreateUserAndLogin([FromBody] NewUser newUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await userRepository.AreCredentialsTaken(newUser.Email, newUser.UserName))
            {
                return BadRequest("Some of your credentials are invalid");
            }

            var user = userFactory.CreateUser(newUser);
            await userRepository.CreateUser(user, newUser.Password, "User");
            user.SessionToken = await userRepository.LoginUser(newUser.Email, newUser.Password);
            return Ok(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginCredentials loginCredentials)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await userRepository.LoginUser(loginCredentials.Email, loginCredentials.Password);
            return Content($"\"{token}\"", "application/json");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("logout"), Authorize(Roles = "Admin, User")]
    public async Task<ActionResult> LogoutUser([FromHeader(Name = "Authorization")] string sessionToken)
    {
        try
        {
            if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }

            sessionToken = sessionToken.Substring("Bearer ".Length).Trim();

            if (!userRepository.IsUserLoggedIn(sessionToken))
            {
                return NotFound("This user could not be found");
            }

            await userRepository.LogoutUser(sessionToken);
            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("GetBySessionToken"), Authorize(Roles = "Admin, User")]
    public async Task<ActionResult<UserDTO>> GetUser([FromHeader(Name = "Authorization")] string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken) || !sessionToken.StartsWith("Bearer "))
        {
            return Unauthorized();
        }

        sessionToken = sessionToken.Substring("Bearer ".Length).Trim();
        var user = await userRepository.GetUserBySessionToken(sessionToken);
        if (user == null)
        {
            throw new ArgumentException($"User of session token: {sessionToken} could not be found");
        }

        return Ok(user.ToDTO());
    }

    [HttpGet("/getUserByUserName")]
    public async Task<ActionResult<UserDTO>> GetUserByUserName(string userName)
    {
        var user = await userRepository.GetUserByUserName(userName);
        if (user == null)
        {
            throw new ArgumentException($"User of username: {userName} could not be found");
        }

        return Ok(user.ToDTO());
    }
  
}