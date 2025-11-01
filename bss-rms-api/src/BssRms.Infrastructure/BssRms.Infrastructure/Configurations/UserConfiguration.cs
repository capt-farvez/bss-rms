using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        // Primary Key
        builder.HasKey(e => e.Uid);

        // Properties
        builder.Property(e => e.Uid)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserName)
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Password)
            .HasMaxLength(500);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.MiddleName)
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FatherName)
            .HasMaxLength(100);

        builder.Property(e => e.MotherName)
            .HasMaxLength(100);

        builder.Property(e => e.SpouseName)
            .HasMaxLength(100);

        builder.Property(e => e.Nid)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Dob)
            .IsRequired();

        builder.Property(e => e.GenderId)
            .IsRequired();

        builder.Property(e => e.Image)
            .HasMaxLength(500);

        builder.Property(e => e.ImageBase64)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.RefreshToken)
            .HasMaxLength(500);

        builder.Property(e => e.RefreshTokenExpiryTime)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(e => e.UserName)
            .IsUnique()
            .HasFilter("[UserName] IS NOT NULL");

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasIndex(e => e.PhoneNumber)
            .IsUnique();

        builder.HasIndex(e => e.Nid)
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.Employee)
            .WithOne(e => e.User)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
