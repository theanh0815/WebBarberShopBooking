using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebBarberShopBooking.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebBarberShopBooking.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class GalleryImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GalleryImagesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/GalleryImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.GalleryImages.ToListAsync());
        }

        // GET: Admin/GalleryImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryImage = await _context.GalleryImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galleryImage == null)
            {
                return NotFound();
            }

            return View(galleryImage);
        }

        // GET: Admin/GalleryImages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/GalleryImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Caption,ImageUrlFile")] GalleryImage galleryImage)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (galleryImage.ImageUrlFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(galleryImage.ImageUrlFile.FileName);
                    string extension = Path.GetExtension(galleryImage.ImageUrlFile.FileName);
                    string uniqueFileName = fileName + "_" + Guid.NewGuid().ToString().Substring(0, 8) + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", uniqueFileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await galleryImage.ImageUrlFile.CopyToAsync(fileStream);
                    }
                    galleryImage.ImageUrl = "/images/" + uniqueFileName;
                }

                _context.Add(galleryImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(galleryImage);
        }

        // GET: Admin/GalleryImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryImage = await _context.GalleryImages.FindAsync(id);
            if (galleryImage == null)
            {
                return NotFound();
            }
            return View(galleryImage);
        }

        // POST: Admin/GalleryImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Caption,ImageUrl,ImageUrlFile")] GalleryImage galleryImage)
        {
            if (id != galleryImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload ảnh (nếu có ảnh mới)
                    if (galleryImage.ImageUrlFile != null)
                    {
                        // Xóa ảnh cũ (nếu có)
                        if (!string.IsNullOrEmpty(galleryImage.ImageUrl) && galleryImage.ImageUrl.StartsWith("/images/"))
                        {
                            string oldImagePath = _hostEnvironment.WebRootPath + galleryImage.ImageUrl;
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(galleryImage.ImageUrlFile.FileName);
                        string extension = Path.GetExtension(galleryImage.ImageUrlFile.FileName);
                        string uniqueFileName = fileName + "_" + Guid.NewGuid().ToString().Substring(0, 8) + extension;
                        string path = Path.Combine(wwwRootPath + "/images/", uniqueFileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await galleryImage.ImageUrlFile.CopyToAsync(fileStream);
                        }
                        galleryImage.ImageUrl = "/images/" + uniqueFileName;
                    }

                    _context.Update(galleryImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GalleryImageExists(galleryImage.Id))
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
            return View(galleryImage);
        }

        // GET: Admin/GalleryImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryImage = await _context.GalleryImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galleryImage == null)
            {
                return NotFound();
            }

            return View(galleryImage);
        }

        // POST: Admin/GalleryImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var galleryImage = await _context.GalleryImages.FindAsync(id);

            // Xóa ảnh (nếu có)
            if (!string.IsNullOrEmpty(galleryImage.ImageUrl) && galleryImage.ImageUrl.StartsWith("/images/"))
            {
                string imagePath = _hostEnvironment.WebRootPath + galleryImage.ImageUrl;
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.GalleryImages.Remove(galleryImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GalleryImageExists(int id)
        {
            return _context.GalleryImages.Any(e => e.Id == id);
        }
    }
}