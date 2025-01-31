using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smooth.Shop.Domain.Models;

namespace Smooth.Shop.Infrastructure.Data.Configuration;

public class NewMediumConfiguration : IEntityTypeConfiguration<NewMedium>
{
    public void Configure(EntityTypeBuilder<NewMedium> builder)
    {
        builder.ToTable("NewMedia", "shop");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.SessionId)
            .IsRequired();

        builder.Property(m => m.OriginalFileName)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();
    }
}
