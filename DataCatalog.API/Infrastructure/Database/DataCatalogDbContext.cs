using DataCatalog.API.Infrastructure.Database.ModelConfiguration;
using DataCatalog.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCatalog.API.Infrastructure.Database
{
    public class DataCatalogDbContext : DbContext
    {
        public const string ConnectionStringName = "DataCatalogDatabase";

        public DbSet<DataSetEntity> DataSets { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        public DataCatalogDbContext(DbContextOptions<DataCatalogDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DataSetEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        }
    }
}
