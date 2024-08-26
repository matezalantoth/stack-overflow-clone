using ElProjectGrande.Models;
using ElProjectGrande.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElProjectGrande.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserRepository userRepository, IUserFactory userFactory) : ControllerBase
{
    [HttpPost("signup")]
    public ActionResult<User> CreateUser([FromBody] NewUser newUser)
    {
        try
        {
            Console.WriteLine("testing");
            var user = userFactory.CreateUser(newUser);
            userRepository.CreateUser(user);
            return Ok(userFactory.CreateUserDTO(user));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
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