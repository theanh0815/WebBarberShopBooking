using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // For [Authorize]
using Microsoft.Extensions.Logging;

namespace WebBarberShopBooking.Controllers
{
    [Authorize] // Yêu cầu người dùng đăng nhập
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, UserManager<User> userManager, ILogger<OrderController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Order/Cart (Hiển thị giỏ hàng)
        public async Task<IActionResult> Cart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var cartItems = await _context.OrderDetails
                    .Include(od => od.Service)
                    .Where(od => od.Order.UserId == user.Id && od.Order.Status == OrderStatus.Pending)
                    .ToListAsync();

                // Kiểm tra xem có đơn hàng Pending nào không, nếu không có thì tạo mới
                var pendingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == OrderStatus.Pending);

                if (pendingOrder == null)
                {
                    pendingOrder = new Order { UserId = user.Id, OrderDate = DateTime.Now, Status = OrderStatus.Pending };
                    _context.Orders.Add(pendingOrder);
                    await _context.SaveChangesAsync();
                }

                ViewBag.OrderId = pendingOrder.Id;

                return View(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị giỏ hàng.");
                return View("Error");
            }
        }

        // POST: Order/Checkout (Xử lý thanh toán)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("Id,ShippingAddress,PaymentMethod")] Order order)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.Status = OrderStatus.Processing; // Chuyển trạng thái sang Processing
                    _context.Update(order);

                    // Tính tổng tiền
                    var cartItems = await _context.OrderDetails
                        .Where(od => od.OrderId == order.Id)
                        .ToListAsync();

                    order.TotalAmount = cartItems.Sum(item => item.Quantity * item.UnitPrice);

                    await _context.SaveChangesAsync();

                    // Xử lý logic thanh toán (ví dụ: tích hợp cổng thanh toán) - **Cần triển khai**
                    // ...

                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction(nameof(History));
                }
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thanh toán.");
                return View("Error");
            }
        }

        // GET: Order/History (Xem lịch sử mua hàng)
        public async Task<IActionResult> History()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var orders = await _context.Orders
                    .Where(o => o.UserId == user.Id && o.Status != OrderStatus.Pending)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem lịch sử mua hàng.");
                return View("Error");
            }
        }

        // GET: Order/Details/5 (Xem chi tiết đơn hàng)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết đơn hàng.");
                return View("Error");
            }
        }
    }
}