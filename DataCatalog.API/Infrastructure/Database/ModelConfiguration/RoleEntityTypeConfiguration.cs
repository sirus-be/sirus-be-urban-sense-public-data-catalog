using DataCatalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DataCatalog.API.Infrastructure.Database.ModelConfiguration
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(idx => idx.Name)
                .IsUnique(false)
                .HasDatabaseName("IDX_Role_Name");
        }
    }
}
