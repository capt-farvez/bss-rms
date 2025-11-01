using BssRms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BssRms.Infrastructure.Configurations;

public class TestTableConfiguration : IEntityTypeConfiguration<TestTable>
{
    public void Configure(EntityTypeBuilder<TestTable> builder)
    {
        builder.ToTable("TestTable");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(e => e.TestDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
