using Microsoft.EntityFrameworkCore;
using CGSpark.Data.Models;

namespace CGSpark.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Admin User", Email = "admin@capgemini.com", Role = "Admin" },
                new User { Id = 2, Name = "Employee A", Email = "employee1@capgemini.com", Role = "Employee" }
            );

            // 🚫 Remove Submission seeding to prevent FK issues if User not found
        }
    }
}
