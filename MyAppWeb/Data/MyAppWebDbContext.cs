//namespace MyAppWeb.Data
//{
//    public class MyAppWebDbContext
//    {
//    }
//}

using Microsoft.EntityFrameworkCore;
using MyAppWeb.Models;

namespace MyAppWeb.Data
{
    public class MyAppWebDbContext : DbContext
    {
        public MyAppWebDbContext(DbContextOptions<MyAppWebDbContext> options) : base(options) { }

        public DbSet<Sheet> Sheets { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sheet>().ToTable("Sheet$");
            modelBuilder.Entity<Employee>().ToTable("Employees");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-PIFD3UM\\SQLEXPRESS;Database=Attendance_Records;Trusted_Connection=True");
        }
    }
}
