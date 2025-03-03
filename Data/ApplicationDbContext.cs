using IsThereAnyoneWho.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IsThereAnyoneWho.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Provision> Provisions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Person
            builder.Entity<Person>()
                .ToTable("Persons");

            // Configure Service
            builder.Entity<Service>()
                .ToTable("Services")
                .HasKey(s => s.Id);

            builder.Entity<Service>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasColumnType("decimal(18,2)");

            // Configure Provision
            builder.Entity<Provision>()
                .ToTable("Provisions")
                .HasKey(p => p.Id);

            builder.Entity<Provision>()
                .Property(p => p.FinalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Provision>()
                .HasOne(p => p.Person)
                .WithMany(person => person.Provisions)
                .HasForeignKey(p => p.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Provision>()
                .HasOne(p => p.Service)
                .WithMany(service => service.Provisions)
                .HasForeignKey(p => p.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
