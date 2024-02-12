using DataCatalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.API.Infrastructure.Database.ModelConfiguration
{
    public class DataSetEntityTypeConfiguration : IEntityTypeConfiguration<DataSetEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DataSetEntity> builder)
        {
            builder.ToTable("DataSets");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.EntityId)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Type)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Data)
                .IsRequired()
                .HasColumnType("jsonb");

            builder.Property(p => p.Context)
           .HasColumnType("jsonb");

            builder.HasMany(b => b.Roles)
               .WithMany(c => c.DataSets)
               .UsingEntity(j => j.ToTable("FactTypeRoles"));
        }
    }
}
