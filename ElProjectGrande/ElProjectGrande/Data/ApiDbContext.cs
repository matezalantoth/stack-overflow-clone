using ElProjectGrande.Models;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MySql");
        Console.WriteLine($"Connection string: {connectionString}");
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
                .HasMaxLength(200);

            entity.Property(q => q.Content)
                .IsRequired()
                .HasMaxLength(500);

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
                .HasMaxLength(500);

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