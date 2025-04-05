using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebBarberShopBooking.Models;
using System.Net.Mail;
using System.Net;

namespace WebBarberShopBooking.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Service)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create  (Không cần thiết, Order thường được tạo từ Checkout)
        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserId,OrderDate,TotalAmount,Status,PaymentMethod,ShippingAddress")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
        //    return View(order);
        //}

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);  // Không cho phép sửa User
            return View(order);
        }

        // POST: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,TotalAmount,Status,PaymentMethod,ShippingAddress")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        // GET: Orders/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            // Logic để lấy thông tin giỏ hàng của người dùng (từ Session, Database, v.v.)
            // Truyền thông tin giỏ hàng vào View để hiển thị
            return View();
        }

        // POST: Orders/PlaceOrder
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string shippingAddress)
        {
            // Logic để lấy thông tin giỏ hàng của người dùng
            var user = await _userManager.GetUserAsync(User);
            var cartItems = GetCartItems(); // Hàm này cần được triển khai để lấy giỏ hàng

            if (cartItems == null || cartItems.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                return View("Checkout"); // Quay lại trang Checkout
            }

            decimal totalAmount = CalculateTotalAmount(cartItems); // Hàm này cần được triển khai

            // Tạo Order mới
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                PaymentMethod = PaymentMethod.COD, // Hoặc PaymentMethod.EmailSent
                ShippingAddress = shippingAddress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo OrderDetails
            foreach (var item in cartItems)
            {
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.Id,
                    ServiceId = item.ServiceId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }
            await _context.SaveChangesAsync();

            ClearCart(); // Xóa giỏ hàng sau khi đặt hàng

            // Gửi email xác nhận (tùy chọn)
            SendOrderConfirmationEmail(user.Email, order.Id);

            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }

        // GET: Orders/OrderConfirmation/5
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // GET: Orders/OrderHistory
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // Các hàm hỗ trợ (cần triển khai)
        private List<CartItem> GetCartItems()
        {
            // Logic để lấy giỏ hàng từ Session, Database, v.v.
            // Ví dụ (Sử dụng Session):
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            return cart ?? new List<CartItem>();
        }

        private void ClearCart()
        {
            // Logic để xóa giỏ hàng (từ Session, Database, v.v.)
            // Ví dụ (Sử dụng Session):
            HttpContext.Session.SetObjectAsJson("Cart", new List<CartItem>());
        }

        private decimal CalculateTotalAmount(List<CartItem> cartItems)
        {
            // Logic để tính tổng tiền từ các CartItem
            decimal total = 0;
            foreach (var item in cartItems)
            {
                total += item.Quantity * item.UnitPrice;
            }
            return total;
        }

        private void SendOrderConfirmationEmail(string userEmail, int orderId)
        {
            // Logic để gửi email xác nhận đơn hàng
            // Sử dụng System.Net.Mail hoặc dịch vụ email bên thứ ba (SendGrid, Mailjet)
            // Ví dụ đơn giản (cần cấu hình SmtpClient):

            var smtpClient = new SmtpClient("your_smtp_host", 587) // Thay đổi thông tin SMTP
            {
                Credentials = new NetworkCredential("your_email@example.com", "your_email_password"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your_email@example.com"),
                Subject = "Xác nhận đơn hàng #" + orderId,
                Body = $"Cảm ơn bạn đã đặt hàng. Đơn hàng của bạn #{orderId} đang được xử lý.",
                IsBodyHtml = true
            };
            mailMessage.To.Add(userEmail);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi gửi email (ghi log, thông báo cho admin, v.v.)
                Console.Error.WriteLine($"Lỗi gửi email: {ex.Message}");
            }
        }
    }

    // Class hỗ trợ cho Cart (cần thiết lập Session)
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class CartItem
    {
        public int? ServiceId { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}