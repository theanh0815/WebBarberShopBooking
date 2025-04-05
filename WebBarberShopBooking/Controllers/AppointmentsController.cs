using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace WebBarberShopBooking.Controllers
{
    [Authorize] // Yêu cầu người dùng đăng nhập để sử dụng
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(ApplicationDbContext context, UserManager<User> userManager, ILogger<AppointmentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Appointment/Index (Hiển thị form đặt lịch)
        public async Task<IActionResult> Index(int? serviceId)
        {
            try
            {
                // Lấy danh sách dịch vụ và thợ để hiển thị trong dropdown
                ViewData["ServiceId"] = new SelectList(await _context.Services.ToListAsync(), "Id", "Name", serviceId);
                ViewData["StylistId"] = new SelectList(await _context.Stylists.ToListAsync(), "Id", "Name");

                // Truyền serviceId (nếu có) để chọn sẵn dịch vụ
                ViewBag.SelectedServiceId = serviceId;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị form đặt lịch.");
                return View("Error");
            }
        }

        // POST: Appointment/Create (Xử lý đặt lịch)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,ServiceId,StylistId,Notes")] Appointment appointment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy thông tin người dùng hiện tại
                    var user = await _userManager.GetUserAsync(User);
                    appointment.UserId = user.Id;
                    appointment.Status = AppointmentStatus.Pending; // Mặc định là Pending

                    _context.Add(appointment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đặt lịch thành công!"; // Thông báo thành công
                    return RedirectToAction(nameof(History)); // Chuyển đến trang lịch sử
                }

                // Nếu ModelState không hợp lệ, trả về form với lỗi
                ViewData["ServiceId"] = new SelectList(await _context.Services.ToListAsync(), "Id", "Name", appointment.ServiceId);
                ViewData["StylistId"] = new SelectList(await _context.Stylists.ToListAsync(), "Id", "Name", appointment.StylistId);
                return View("Index", appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lịch.");
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Appointment/History (Xem lịch sử đặt lịch)
        public async Task<IActionResult> History()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var appointments = await _context.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Stylist)
                    .Where(a => a.UserId == user.Id)
                    .OrderByDescending(a => a.DateTime)
                    .ToListAsync();

                return View(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem lịch sử đặt lịch.");
                return View("Error");
            }
        }

        // GET: Appointment/Details/5 (Xem chi tiết lịch hẹn)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.Stylist)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointment/Cancel/5 (Hiển thị xác nhận hủy lịch)
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Stylist)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointment/Cancel/5 (Xử lý hủy lịch)
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int? id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment != null)
                {
                    appointment.Status = AppointmentStatus.Cancelled;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hủy lịch thành công!";
                    return RedirectToAction(nameof(History));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hủy lịch.");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy lịch. Vui lòng thử lại.";
                return RedirectToAction(nameof(History));
            }
        }
    }
}