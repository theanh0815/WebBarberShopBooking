using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu thống kê
            var totalUsers = await _context.Users.CountAsync();
            var totalServices = await _context.Services.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();

            // Tính doanh thu (ví dụ: tổng tiền các đơn hàng đã hoàn thành)
            var completedOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Completed).ToListAsync();
            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);

            // Truyền dữ liệu vào ViewBag hoặc ViewModel
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalServices = totalServices;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TotalRevenue = totalRevenue;

            return View();
        }

        // Các action khác cho thống kê chi tiết hơn (ví dụ: doanh thu theo tháng, dịch vụ được đặt nhiều nhất, v.v.)
    }
}