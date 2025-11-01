using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class EmployeeTableConfiguration : IEntityTypeConfiguration<EmployeeTable>
{
    public void Configure(EntityTypeBuilder<EmployeeTable> builder)
    {
        builder.ToTable("EmployeeTable");

        // Primary Key
        builder.HasKey(e => e.EmployeeTableId);

        // Properties
        builder.Property(e => e.EmployeeTableId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.Property(e => e.TableId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(e => new { e.EmployeeId, e.TableId })
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.Employee)
            .WithMany(emp => emp.EmployeeTables)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Table)
            .WithMany(t => t.EmployeeTables)
            .HasForeignKey(e => e.TableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
