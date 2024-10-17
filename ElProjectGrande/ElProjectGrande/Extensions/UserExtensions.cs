using ElProjectGrande.Models;
using ElProjectGrande.Models.UserModels;
using ElProjectGrande.Models.UserModels.DTOs;
using UserDTO = ElProjectGrande.Models.UserModels.DTOs.UserDTO;

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
            Karma = user.Karma, Upvotes = upvotes, Downvotes = downvotes, MutedUntil = user.MutedUntil, Muted = user.Muted,
            Banned = user.Banned,
        };
    }

    public static PublicUserDTO ToPublicDTO(this User user)
    {
        return new PublicUserDTO
        {
            UserName = user.UserName, Karma = user.Karma, Answers = user.Answers.Select(a => a.ToDTO()).ToList(),
            Questions = user.Questions.Select(q => q.ToDTO()).ToList(), MutedUntil = user.MutedUntil, Muted = user.Muted,
            Banned = user.Banned
        };
    }
}