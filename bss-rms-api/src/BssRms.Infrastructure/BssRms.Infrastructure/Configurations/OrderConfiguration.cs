using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");

        // Primary Key
        builder.HasKey(e => e.OrderId);

        // Properties
        builder.Property(e => e.OrderId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.TableId)
            .IsRequired();

        builder.Property(e => e.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.OrderDate)
            .IsRequired();

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(e => e.OrderNumber)
            .IsUnique();

        builder.HasIndex(e => e.OrderDate);

        builder.HasIndex(e => e.Status);

        // Relationships
        builder.HasOne(e => e.Table)
            .WithMany(t => t.Orders)
            .HasForeignKey(e => e.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
