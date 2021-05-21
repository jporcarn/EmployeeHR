using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.EF
{
    public class EmployeeHRDbContext : DbContext
    {
        public DbSet<Employee> Employee { get; set; }

        public EmployeeHRDbContext(DbContextOptions<EmployeeHRDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // optionsBuilder.UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;User ID='';Initial Catalog=EmmployeeHRDb;Data Source=.\\sqlexpress;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .IsRequired();

                entity.Property(e => e.SocialSecurityNumber)
                    .IsRequired();

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .IsConcurrencyToken();
            });
        }
    }
}