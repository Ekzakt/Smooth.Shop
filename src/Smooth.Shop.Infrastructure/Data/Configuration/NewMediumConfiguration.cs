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

        builder.Property(m => m.UserId)
            .HasColumnOrder(1)
            .IsRequired();

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasColumnOrder(2);

        builder.Property(m => m.Status)
            .HasMaxLength(50)
            .HasColumnOrder(3)
            .IsRequired();

        builder.Property(m => m.ConnectionId)
            .HasColumnOrder(4)
            .IsRequired();

        builder.Property(m => m.OriginalFileName)
            .HasColumnOrder(5)
            .IsRequired();

        builder.Property(m => m.UploadedFileName)
            .HasColumnOrder(6)
            .IsRequired();

        builder.Property(m => m.FileSize)
            .HasColumnOrder(7)
            .IsRequired();

        builder.Property(m => m.UploadStartedAt)
            .HasColumnOrder(8)
            .IsRequired();

        builder.Property(m => m.UploadFinishedAt)
            .HasColumnOrder(9)
            .IsRequired();

        builder.Property(m => m.UploadTimeMs)
            .HasColumnOrder(10)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .HasColumnOrder(11)
            .IsRequired();

        builder.Property(m => m.UserId)
            .HasColumnOrder(12)
            .IsRequired();
    }
}
