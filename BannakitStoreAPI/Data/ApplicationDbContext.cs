using BannakitStoreApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BannakitStoreApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PaymentType> paymentTypes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ตั้งค่าความสัมพันธ์ระหว่าง User และ Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)  // User มี Role
                .WithMany(r => r.Users)  // Role มีหลาย Users
                .HasForeignKey(u => u.RoleId);  // กำหนด RoleId เป็น Foreign Key
        }
    }
}
