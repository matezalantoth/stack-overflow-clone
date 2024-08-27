using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserRepository userRepository, IUserFactory userFactory) : ControllerBase
{
    [HttpPost("signup")]
    public ActionResult<Guid> CreateUserAndLogin([FromBody] NewUser newUser)
    {
        try
        {
            var user = userFactory.CreateUser(newUser);
            userRepository.CreateUser(user);
            return Ok(user.SessionToken);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
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