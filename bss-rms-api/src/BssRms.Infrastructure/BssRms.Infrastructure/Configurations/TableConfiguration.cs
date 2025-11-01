using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Table");

        // Primary Key
        builder.HasKey(e => e.TableId);

        // Properties
        builder.Property(e => e.TableId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.TableNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NumberOfSeats)
            .IsRequired();

        builder.Property(e => e.Image)
            .HasMaxLength(500);

        builder.Property(e => e.ImageBase64)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(e => e.TableNumber)
            .IsUnique();

        // Relationships
        builder.HasMany(e => e.EmployeeTables)
            .WithOne(et => et.Table)
            .HasForeignKey(et => et.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Orders)
            .WithOne(o => o.Table)
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
