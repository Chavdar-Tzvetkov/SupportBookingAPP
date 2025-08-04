using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupportBookingAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SupportCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupportCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SupportCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.SupportCategories.ToListAsync());
        }

        // GET: SupportCategories/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportCategory = await _context.SupportCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supportCategory == null)
            {
                return NotFound();
            }

            return View(supportCategory);
        }

        // GET: SupportCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SupportCategories/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SupportCategory supportCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supportCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supportCategory);
        }

        // GET: SupportCategories/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportCategory = await _context.SupportCategories.FindAsync(id);
            if (supportCategory == null)
            {
                return NotFound();
            }
            return View(supportCategory);
        }

        // POST: SupportCategories/Edit/
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SupportCategory supportCategory)
        {
            if (id != supportCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supportCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupportCategoryExists(supportCategory.Id))
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
            return View(supportCategory);
        }

        // GET: SupportCategories/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supportCategory = await _context.SupportCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supportCategory == null)
            {
                return NotFound();
            }

            return View(supportCategory);
        }

        // POST: SupportCategories/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supportCategory = await _context.SupportCategories.FindAsync(id);
            if (supportCategory != null)
            {
                _context.SupportCategories.Remove(supportCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupportCategoryExists(int id)
        {
            return _context.SupportCategories.Any(e => e.Id == id);
        }
    }
}
