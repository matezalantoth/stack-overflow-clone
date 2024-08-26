namespace ElProjectGrande.Services;

public interface IUserVerifier
{
    public string HashPassword(string password, out byte[] salt);

    public bool VerifyPassword(string password, string hash, byte[] salt);
}