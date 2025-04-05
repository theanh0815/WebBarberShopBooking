using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.Extensions.Logging;

namespace WebBarberShopBooking.Controllers
{
    public class StylistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StylistController> _logger;

        public StylistController(ApplicationDbContext context, ILogger<StylistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Stylist/Index (Hiển thị danh sách thợ)
        public async Task<IActionResult> Index()
        {
            try
            {
                var stylists = await _context.Stylists.ToListAsync();
                return View(stylists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách thợ.");
                return View("Error");
            }
        }

        // GET: Stylist/Details/5 (Xem chi tiết thợ)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var stylist = await _context.Stylists.FirstOrDefaultAsync(m => m.Id == id);
                if (stylist == null)
                {
                    return NotFound();
                }
                return View(stylist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết thợ.");
                return View("Error");
            }
        }

        // GET: Stylist/Create (Thêm thợ - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stylist/Create (Xử lý thêm thợ - Chỉ Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Bio,ImageUrl")] Stylist stylist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stylist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stylist);
        }

        // GET: Stylist/Edit/5 (Sửa thợ - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stylist = await _context.Stylists.FindAsync(id);
            if (stylist == null)
            {
                return NotFound();
            }
            return View(stylist);
        }

        // POST: Stylist/Edit/5 (Xử lý sửa thợ - Chỉ Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Bio,ImageUrl")] Stylist stylist)
        {
            if (id != stylist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stylist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StylistExists(stylist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(stylist);
        }

        // GET: Stylist/Delete/5 (Xóa thợ - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stylist = await _context.Stylists.FirstOrDefaultAsync(m => m.Id == id);
            if (stylist == null)
            {
                return NotFound();
            }

            return View(stylist);
        }

        // POST: Stylist/Delete/5 (Xử lý xóa thợ - Chỉ Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var stylist = await _context.Stylists.FindAsync(id);
            _context.Stylists.Remove(stylist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StylistExists(int id)
        {
            return _context.Stylists.Any(e => e.Id == id);
        }
    }
}