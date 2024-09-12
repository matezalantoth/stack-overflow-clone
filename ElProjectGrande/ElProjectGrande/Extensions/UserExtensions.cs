using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;

namespace ElProjectGrande.Extensions;

public static class UserExtensions
{
    public static UserDTO ToDTO(this User user)
    {
        var upvotes = new List<Guid>();
        var downvotes = new List<Guid>();

        if (user.Upvotes != null && user.Upvotes.Count != 0)
        {
            upvotes = user.Upvotes.Select(upvote => upvote).ToList();
        }

        if (user.Downvotes != null && user.Downvotes.Count != 0)
        {
            downvotes = user.Downvotes.Select(upvote => upvote).ToList();
        }

        return new UserDTO
        {
            UserName = user.UserName, Name = user.Name, SessionToken = user.SessionToken,
            Answers = user.Answers.Select(answer => answer.ToDTO()).ToList(),
            Email = user.Email, Questions = user.Questions.Select(question => question.ToDTO()).ToList(),
            Karma = user.Karma, Upvotes = upvotes, Downvotes = downvotes
        };
    }
}