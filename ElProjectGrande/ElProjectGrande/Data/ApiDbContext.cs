using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace ElProjectGrande.Data;

public class ApiDbContext(DbContextOptions<ApiDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.DoB)
                .IsRequired();

            entity.Property(u => u.SessionToken)
                .IsRequired();

            entity.Property(u => u.Karma)
                .IsRequired();

            entity.HasMany(u => u.Questions)
                .WithOne(q => q.User)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Answers)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            entity.Property(u => u.Karma)
                .IsRequired();

            var guidListConverter = new ValueConverter<List<Guid>, string>(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()
            );

            var guidListComparer = new ValueComparer<List<Guid>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            entity.Property(u => u.Upvotes)
                .HasConversion(guidListConverter)
                .Metadata.SetValueComparer(guidListComparer);

            entity.Property(u => u.Downvotes)
                .HasConversion(guidListConverter)
                .Metadata.SetValueComparer(guidListComparer);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.HasIndex(q => q.Id).IsUnique();

            entity.Property(q => q.Title)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(q => q.Content)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(q => q.PostedAt)
                .IsRequired();

            entity.HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId);

            entity.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.HasIndex(a => a.Id).IsUnique();

            entity.Property(a => a.Content)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(a => a.PostedAt)
                .IsRequired();

            entity.Property(a => a.Accepted)
                .IsRequired();

            entity.HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId);

            entity.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(a => a.Votes)
                .IsRequired();
        });
    }
}