using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLapi.Domain.Entities;

namespace URLapi.Infra.Mapping;

public class UrlMap : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.ToTable("Urls");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.LongUrl)
            .HasColumnName("LongUrl")
            .HasColumnType("VARCHAR")
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(x => x.ShortUrl)
            .HasColumnName("ShortUrl")
            .HasColumnType("VARCHAR")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.ShortUrl)
            .IsUnique();

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey("CreatorId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(x => x.ModifiedAt)
            .HasColumnName("ModifiedAt")
            .IsRequired();

        builder.Property(x => x.AccessCount)
            .HasColumnName("AccessCount")
            .IsRequired();
    }
}