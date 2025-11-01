using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee");

        // Primary Key
        builder.HasKey(e => e.EmployeeId);

        // Properties
        builder.Property(e => e.EmployeeId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Designation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.JoinDate)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(e => e.UserId)
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.User)
            .WithOne(e => e.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EmployeeTables)
            .WithOne(et => et.Employee)
            .HasForeignKey(et => et.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
