namespace BackendServer.Models.UserModels.DTOs;

public class NewUser
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime DoB { get; set; }
};