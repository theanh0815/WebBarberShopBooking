using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet cho các Models của bạn
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Stylist> Stylists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Định nghĩa các ràng buộc và mối quan hệ (nếu cần)
            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Khi User bị xóa, các Appointment của họ cũng bị xóa

            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Cascade); // Khi Service bị xóa, các Appointment liên quan cũng bị xóa (tùy chọn)

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId)
                .OnDelete(DeleteBehavior.SetNull); // Khi Service bị xóa, ServiceId trong OrderDetail sẽ 

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany() // User có thể có nhiều Review
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany() // Service có thể có nhiều Review
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<Appointment>()
                .HasOne(a => a.Stylist)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StylistId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
