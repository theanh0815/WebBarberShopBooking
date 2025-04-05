using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.Extensions.Logging;

namespace WebBarberShopBooking.Controllers
{
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(ApplicationDbContext context, ILogger<PromotionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Promotion/Index (Hiển thị danh sách khuyến mãi)
        public async Task<IActionResult> Index()
        {
            try
            {
                var promotions = await _context.Promotions.ToListAsync();
                return View(promotions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách khuyến mãi.");
                return View("Error");
            }
        }

        // GET: Promotion/Details/5 (Xem chi tiết khuyến mãi)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var promotion = await _context.Promotions.FirstOrDefaultAsync(m => m.Id == id);
                if (promotion == null)
                {
                    return NotFound();
                }
                return View(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết khuyến mãi.");
                return View("Error");
            }
        }

        // GET: Promotion/Create (Thêm khuyến mãi - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Promotion/Create (Xử lý thêm khuyến mãi - Chỉ Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,StartDate,EndDate,DiscountPercentage,DiscountAmount")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Promotion/Edit/5 (Sửa khuyến mãi - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        // POST: Promotion/Edit/5 (Xử lý sửa khuyến mãi - Chỉ Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,DiscountPercentage,DiscountAmount")] Promotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.Id))
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
            return View(promotion);
        }

        // GET: Promotion/Delete/5 (Xóa khuyến mãi - Chỉ Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // POST: Promotion/Delete/5 (Xử lý xóa khuyến mãi - Chỉ Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) {
                return NotFound();
            }
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.Id == id);
        }
    }
}