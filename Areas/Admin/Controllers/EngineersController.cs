using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EngineersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EngineersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Engineers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Engineers.ToListAsync());
        }

        // GET: Engineers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var engineer = await _context.Engineers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (engineer == null)
            {
                return NotFound();
            }

            return View(engineer);
        }

        // GET: Engineers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Engineers/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Specialty,WorkdayStart,WorkdayEnd")] Engineer engineer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(engineer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(engineer);
        }

        // GET: Engineers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var engineer = await _context.Engineers.FindAsync(id);
            if (engineer == null)
            {
                return NotFound();
            }
            return View(engineer);
        }

        // POST: Engineers/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Specialty,WorkdayStart,WorkdayEnd")] Engineer engineer)
        {
            if (id != engineer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(engineer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EngineerExists(engineer.Id))
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
            return View(engineer);
        }

        // GET: Engineers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var engineer = await _context.Engineers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (engineer == null)
            {
                return NotFound();
            }

            return View(engineer);
        }

        // POST: Engineers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var engineer = await _context.Engineers.FindAsync(id);
            if (engineer != null)
            {
                _context.Engineers.Remove(engineer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EngineerExists(int id)
        {
            return _context.Engineers.Any(e => e.Id == id);
        }
    }
}
