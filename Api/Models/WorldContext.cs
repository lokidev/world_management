using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace WorldManagementApi.Models
{
    public partial class WorldContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public WorldContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public WorldContext(DbContextOptions<WorldContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<World> Worlds { get; set; }
        public virtual DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetSection("SqlConnection").Get<string>());
                // Use when running outside of docker
                // optionsBuilder.UseSqlServer("Server=localhost,5434;Database=World;User ID=sa;Password=Yukon900;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<World>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("World");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("People");
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.DestructionDate).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IdentificationTags).IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
