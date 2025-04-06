using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http; // For IFormFile
using System.IO; // For file operations

namespace WebBarberShopBooking.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServiceController> _logger;
        private readonly string _imagePath = "wwwroot/images"; // Đường dẫn lưu ảnh

        public ServiceController(IServiceRepository serviceRepository, ILogger<ServiceController> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        // GET: Service/Index
        [Route("/Services")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var services = await _serviceRepository.GetAllServicesAsync();
                return View(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách dịch vụ.");
                return View("Error");
            }
        }

        // GET: Service/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service == null)
                {
                    return NotFound();
                }

                // Giả sử bạn cần lấy reviews, bạn có thể thêm logic này vào repository hoặc service
                //var reviews = await _context.Reviews
                //    .Include(r => r.User)
                //    .Where(r => r.ServiceId == id)
                //    .ToListAsync();

                // ViewData["Reviews"] = reviews;

                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem chi tiết dịch vụ.");
                return View("Error");
            }
        }

        // GET: Service/Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageFile")] Service service)
        {
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Xử lý ảnh
                        if (service.ImageFile != null && service.ImageFile.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(service.ImageFile.FileName);
                            string filePath = Path.Combine(_imagePath, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await service.ImageFile.CopyToAsync(fileStream);
                            }
                            service.ImageUrl = "/images/" + fileName;
                        }
                        else
                        {
                            service.ImageUrl = "/images/default.png"; // Hình ảnh mặc định
                        }
                        await _serviceRepository.AddServiceAsync(service);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi tạo dịch vụ.", ex);
                        return View("Error");
                    }
                }

            }
            
             return View(service);
        }

        // GET: Service/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service == null)
                {
                    return NotFound();
                }
                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang sửa dịch vụ.", ex);
                return View("Error");
            }
        }

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageFile")] Service service)
        {
            if (id != service.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingService = await _serviceRepository.GetServiceByIdAsync(id);
                    if (existingService == null)
                    {
                        return NotFound();
                    }

                    // Xử lý ảnh
                    if (service.ImageFile != null && service.ImageFile.Length > 0)
                    {
                        // Xóa ảnh cũ
                        if (!string.IsNullOrEmpty(existingService.ImageUrl) && existingService.ImageUrl != "/images/default.png")
                        {
                            string oldFilePath = Path.Combine("wwwroot", existingService.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Lưu ảnh mới
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(service.ImageFile.FileName);
                        string filePath = Path.Combine(_imagePath, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await service.ImageFile.CopyToAsync(fileStream);
                        }
                        service.ImageUrl = "/images/" + fileName;
                    }
                    else
                    {
                        service.ImageUrl = existingService.ImageUrl; // Giữ nguyên ảnh cũ
                    }

                    service.Id = id; // Đảm bảo ID được gán đúng
                    await _serviceRepository.UpdateServiceAsync(service);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await _serviceRepository.ServiceExistsAsync(service.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Lỗi đồng thời khi cập nhật dịch vụ.", ex);
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật dịch vụ.", ex);
                    return View(service);
                }
            }
            return View(service);
        }

        // GET: Service/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service == null)
                {
                    return NotFound();
                }
                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang xóa dịch vụ.", ex);
                return View("Error");
            }
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service != null)
                {
                    // Xóa ảnh
                    if (!string.IsNullOrEmpty(service.ImageUrl) && service.ImageUrl != "/images/default.png")
                    {
                        string imagePath = Path.Combine("wwwroot", service.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    await _serviceRepository.DeleteServiceAsync(id.Value);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dịch vụ.", ex);
                return View("Error");
            }
        }

        private bool ServiceExists(int id)
        {
            return _serviceRepository.ServiceExistsAsync(id).Result; // Or use .GetAwaiter().GetResult() with caution
        }
    }
}