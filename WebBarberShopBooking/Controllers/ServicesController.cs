﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBarberShopBooking.Models;
using WebBarberShopBooking.Repositories;

namespace WebBarberShopBooking.Controllers {
    public class ServiceController : Controller {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServiceController> _logger;
        private readonly string _imagePath = "wwwroot/images"; // Đường dẫn lưu ảnh

        public ServiceController(IServiceRepository serviceRepository, ILogger<ServiceController> logger) {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        // GET: Service/Index
        [Route("/Services")]
        public async Task<IActionResult> Index() {
            try {
                var services = await _serviceRepository.GetAllServicesAsync();
                return View(services);
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi lấy danh sách dịch vụ.");
                return View("Error");
            }
        }

        // GET: Service/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            try {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service == null) {
                    return NotFound();
                }

                // Giả sử bạn cần lấy reviews, bạn có thể thêm logic này vào repository hoặc service
                //var reviews = await _context.Reviews
                //    .Include(r => r.User)
                //    .Where(r => r.ServiceId == id)
                //    .ToListAsync();

                // ViewData["Reviews"] = reviews;

                return View(service);
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi xem chi tiết dịch vụ.");
                return View("Error");
            }
        }

        // GET: Service/Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create() {
            return View();
        }

        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Service service, IFormFile ImageUrl) {
            try {
                if (ImageUrl != null) {
                    var extension = Path.GetExtension(ImageUrl.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    service.ImageUrl = await SaveImage(ImageUrl, fileName);
                }
                await _serviceRepository.AddServiceAsync(service);
                return RedirectToAction(nameof(Index));
            } catch {
                return View(service);
            }
        }
        private async Task<string> SaveImage(IFormFile image, string fileName) {
            var savePath = Path.Combine(_imagePath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create)) {
                await image.CopyToAsync(fileStream);
            }
            return "images/" + fileName;
        }

        // GET: Service/Edit/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id) {
            try {
                var service = await _serviceRepository.GetServiceByIdAsync(id);
                if (service == null) {
                    return NotFound();
                }
                return View(service);
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi hiển thị trang sửa dịch vụ.", ex);
                return View("Error");
            }
        }

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Service service, IFormFile ImageFile) {
            if (id != service.Id) {
                return NotFound();
            }

            try {
                if (ImageFile != null) {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(service.ImageUrl) && service.ImageUrl != "/images/default.png") {
                        string oldImagePath = Path.Combine("wwwroot", service.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    // Lưu ảnh mới
                    var extension = Path.GetExtension(ImageFile.FileName);
                    var fileName = Guid.NewGuid().ToString() + extension;
                    service.ImageUrl = await SaveImage(ImageFile, fileName);
                }
                await _serviceRepository.UpdateServiceAsync(service);
                return RedirectToAction(nameof(Index));
            } catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(ex, "Lỗi đồng thời khi cập nhật dịch vụ.", ex);
                return View("Error");
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi cập nhật dịch vụ.", ex);
                return View(service);
            }
        }

        // GET: Service/Delete/5
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            try {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service == null) {
                    return NotFound();
                }
                return View(service);
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi hiển thị trang xóa dịch vụ.", ex);
                return View("Error");
            }
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id) {
            try {
                var service = await _serviceRepository.GetServiceByIdAsync(id.Value);
                if (service != null) {
                    // Xóa ảnh
                    if (!string.IsNullOrEmpty(service.ImageUrl) && service.ImageUrl != "/images/default.png") {
                        string imagePath = Path.Combine("wwwroot", service.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath)) {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    await _serviceRepository.DeleteServiceAsync(id.Value);
                }

                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi xóa dịch vụ.", ex);
                return View("Error");
            }
        }
    }
}