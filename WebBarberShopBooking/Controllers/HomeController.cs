using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy dữ liệu cần thiết cho trang chủ (ví dụ: dịch vụ nổi bật, sản phẩm mới, tin tức mới nhất)
            var featuredServices = _context.Services.Take(3).ToList();
            var newProducts = _context.Products.OrderByDescending(p => p.Id).Take(3).ToList();
            var latestNews = _context.News.OrderByDescending(n => n.PublishDate).Take(3).ToList();

            ViewBag.FeaturedServices = featuredServices;
            ViewBag.NewProducts = newProducts;
            ViewBag.LatestNews = latestNews;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}