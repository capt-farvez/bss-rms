using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem");

        // Primary Key
        builder.HasKey(e => e.OrderItemId);

        // Properties
        builder.Property(e => e.OrderItemId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.OrderId)
            .IsRequired();

        builder.Property(e => e.FoodId)
            .IsRequired();

        builder.Property(e => e.Quantity)
            .IsRequired();

        builder.Property(e => e.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Indexes
        builder.HasIndex(e => e.OrderId);

        builder.HasIndex(e => e.FoodId);

        // Relationships
        builder.HasOne(e => e.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Food)
            .WithMany()
            .HasForeignKey(e => e.FoodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
