using ElProjectGrande.Models;
using ElProjectGrande.Models.AnswerModels;
using ElProjectGrande.Models.QuestionModels;
using ElProjectGrande.Models.TagModels;
using ElProjectGrande.Models.UserModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ElProjectGrande.Data;

using Microsoft.EntityFrameworkCore;
using System;

public class ApiDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    
    public DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MySql");
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            connectionString = $"Server={Environment.GetEnvironmentVariable("LOCAL_SERVER_NAME")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"User={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_USER_PASSWORD")};" +
                               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};";
        }

        optionsBuilder.UseMySQL(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Id).IsUnique();

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Password)
                .IsRequired();

            entity.Property(u => u.Salt)
                .IsRequired();

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
            
            entity.HasMany(q => q.Tags)
                .WithMany(t => t.Questions)
                .UsingEntity(
                    "QuestionTag",
                    r => r.HasOne(typeof(Question)).WithMany().HasForeignKey("QuestionsId").HasPrincipalKey(nameof(Question.Id)),
                    l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                    j => j.HasKey("QuestionsId", "TagsId"));
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

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            
            entity.HasIndex(t => t.Id).IsUnique();

            entity.Property(t => t.TagName)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.HasMany(t => t.Questions)
                .WithMany(q => q.Tags)
                .UsingEntity("QuestionTag",
                    r => r.HasOne(typeof(Question)).WithMany().HasForeignKey("QuestionsId").HasPrincipalKey(nameof(Question.Id)),
                    l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
                    j => j.HasKey("QuestionsId", "TagsId"));
        });
    }
}