namespace ElProjectGrande.Models;

public class NewQuestion
{
    public string Content { get; set; }
    public string Title { get; set; }
    public Guid SessionToken { get; set; }
    public DateTime PostedAt { get; set; }
}