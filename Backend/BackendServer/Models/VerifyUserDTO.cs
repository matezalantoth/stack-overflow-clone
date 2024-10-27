namespace BackendServer.Models;

public class VerifyUserDTO
{
    public string Email { get; set; }
    
    public bool Verified { get; set; }
}