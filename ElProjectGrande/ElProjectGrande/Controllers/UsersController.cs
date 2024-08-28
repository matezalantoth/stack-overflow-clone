using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserRepository userRepository, IUserFactory userFactory, IUserVerifier userVerifier)
    : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<Guid>> CreateUserAndLogin([FromBody] NewUser newUser)
    {
        try
        {
            if (await userRepository.AreCredentialsTaken(newUser.Email, newUser.UserName))
            {
                return BadRequest("Some of your credentials are invalid");
            }

            var user = userFactory.CreateUser(newUser);
            userRepository.CreateUser(user);
            return Ok(user.SessionToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<Guid>> Login([FromBody] LoginCredentials loginCredentials)
    {
        try
        {
            var user = await userRepository.GetUserByEmail(loginCredentials.Email);
            if (user == null || !userVerifier.VerifyPassword(loginCredentials.Password, user.Password, user.Salt))
            {
                return BadRequest("Some of your credentials are incorrect");
            }

            return Ok(userRepository.LoginUser(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("logout")]
    public ActionResult LogoutUser([FromHeader(Name = "Authorization")] Guid sessionToken)
    {
        try
        {
            if (sessionToken == Guid.Empty)
            {
                throw new ArgumentException("This session token does not exist");
            }

            if (!userRepository.IsUserLoggedIn(sessionToken))
            {
                return NotFound("This user could not be found");
            }

            userRepository.LogoutUser(sessionToken);
            return Ok();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetUser(Guid userId)
    {
        var user = await userRepository.GetUserById(userId);
        if (user == null)
        {
            throw new ArgumentException($"User of id: {userId} could not be found");
        }

        return Ok(userFactory.CreateUserDTO(user));
    }
}