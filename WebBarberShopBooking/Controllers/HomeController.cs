using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebBarberShopBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy tin tức mới nhất (tối đa 3 tin)
                var latestNews = await _context.News
                    .OrderByDescending(n => n.PublishDate)
                    .Take(3)
                    .ToListAsync();

                // Lấy dịch vụ nổi bật (ví dụ: có nhiều đánh giá tốt)
                var featuredServices = await _context.Services.Take(4).ToListAsync(); // Đơn giản là lấy 4 dịch vụ đầu tiên

                // Truyền dữ liệu vào View
                ViewData["LatestNews"] = latestNews;
                ViewData["FeaturedServices"] = featuredServices;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu cho trang chủ.");
                return View("Error"); // Hoặc chuyển hướng đến trang thông báo lỗi
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Contact()
        //{
        //    return
        //}
    }

}