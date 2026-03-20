using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infra.Mapping;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(120)
            .IsRequired();

        builder.OwnsOne(x => x.PasswordHash, password =>
        {
            password.Property(p => p.Hash)
                .HasColumnName("PasswordHash")
                .IsRequired();
        });

        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName("Email")
                .IsRequired();

            email.OwnsOne(e => e.Verification, verification =>
            {
                verification.Property(v => v.VerifyHashCode)
                    .HasColumnName("EmailVerificationCode")
                    .IsRequired();

                verification.Property(v => v.VerifyHashCodeExpiration)
                    .HasColumnName("EmailVerificationExpiration")
                    .IsRequired();

                verification.Property(v => v.Verified)
                    .HasColumnName("EmailVerification")
                    .IsRequired();
            });
        });
    }
}