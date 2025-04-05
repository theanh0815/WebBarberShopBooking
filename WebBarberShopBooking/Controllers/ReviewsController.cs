using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebBarberShopBooking.Models;

namespace WebBarberShopBooking.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reviews.Include(r => r.User).Include(r => r.Service);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize]
        public IActionResult Create(int? serviceId, int? productId)
        {
            ViewBag.ServiceId = serviceId;
            ViewBag.ProductId = productId;
            return View();
        }

        // POST: Reviews/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceId,ProductId,Rating,Comment")] Review review)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                review.UserId = user.Id;
                review.CreatedAt = DateTime.Now;

                _context.Add(review);
                await _context.SaveChangesAsync();

                if (review.ServiceId.HasValue)
                {
                    return RedirectToAction("Details", "Services", new { id = review.ServiceId });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Hoặc RedirectToAction("Index", "Home");
            }

            return View(review);
        }

        // POST: Reviews/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rating,Comment")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReview = await _context.Reviews.FindAsync(id);
                    if (existingReview.UserId != _userManager.GetUserId(User))
                    {
                        return Forbid(); // Hoặc RedirectToAction("Index", "Home");
                    }

                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;

                    _context.Update(existingReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (review.ServiceId.HasValue)
                {
                    return RedirectToAction("Details", "Services", new { id = review.ServiceId });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Hoặc RedirectToAction("Index", "Home");
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Hoặc RedirectToAction("Index", "Home");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            if (review.ServiceId.HasValue)
            {
                return RedirectToAction("Details", "Services", new { id = review.ServiceId });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}