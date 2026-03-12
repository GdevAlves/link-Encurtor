﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLapi.Domain.Entities;

namespace URLapi.Infra.Mapping;

public class UrlAccessLogMap : IEntityTypeConfiguration<UrlAccessLog>
{
    public void Configure(EntityTypeBuilder<UrlAccessLog> builder)
    {
        builder.ToTable("UrlAccessLog");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UrlId)
            .IsRequired();

        builder.HasOne(x => x.Url)
            .WithMany()
            .HasForeignKey(x => x.UrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}