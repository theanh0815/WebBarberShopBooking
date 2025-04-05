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
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.Service).Include(a => a.User).Include(a => a.Stylist);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.User)
                .Include(a => a.Stylist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ServiceId"] = _context.Services.ToList();
            ViewData["StylistId"] = _context.Stylists.ToList();
            return View();
        }

        // POST: Appointments/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,ServiceId,StylistId,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                appointment.UserId = user.Id;

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home"); // Hoặc trang xác nhận
            }
            ViewData["ServiceId"] = _context.Services.ToList();
            ViewData["StylistId"] = _context.Stylists.ToList();
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = _context.Services.ToList();
            ViewData["StylistId"] = _context.Stylists.ToList();
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,ServiceId,StylistId,UserId,Notes,Status")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["ServiceId"] = _context.Services.ToList();
            ViewData["StylistId"] = _context.Stylists.ToList();
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        // GET: Appointments/UserAppointments
        [Authorize]
        public async Task<IActionResult> UserAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            var appointments = await _context.Appointments
                .Where(a => a.UserId == user.Id)
                .Include(a => a.Service)
                .Include(a => a.Stylist)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointments/Cancel/5
        [Authorize]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // Hoặc RedirectToAction("Index", "Home");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserAppointments));
        }
    }
}