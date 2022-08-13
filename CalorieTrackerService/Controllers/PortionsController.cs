using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcCalorie.Data;
using MvcCalorie.Models;

namespace MvcCalorie.Controllers
{
    public class PortionsController : Controller
    {
        private readonly MvcCalorieContext _context;

        public PortionsController(MvcCalorieContext context)
        {
            _context = context;
        }

        // GET: Portions
        public async Task<IActionResult> Index()
        {
            return _context.Portion != null ? 
                          View(await _context.Portion.ToListAsync()) :
                          Problem("Entity set 'MvcCalorieContext.Portion'  is null.");
        }

        // GET: Portions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Portion == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (portion == null)
            {
                return NotFound();
            }

            return View(portion);
        }

        // GET: Portions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Portions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,Product,Calories,UserToken")] Portion portion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(portion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(portion);
        }

        // GET: Portions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Portion == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion.FindAsync(id);
            if (portion == null)
            {
                return NotFound();
            }
            return View(portion);
        }

        // POST: Portions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,Product,Calories,UserToken")] Portion portion)
        {
            if (id != portion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(portion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortionExists(portion.Id))
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
            return View(portion);
        }

        // GET: Portions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Portion == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (portion == null)
            {
                return NotFound();
            }

            return View(portion);
        }

        // POST: Portions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Portion == null)
            {
                return Problem("Entity set 'MvcCalorieContext.Portion'  is null.");
            }
            var portion = await _context.Portion.FindAsync(id);
            if (portion != null)
            {
                _context.Portion.Remove(portion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortionExists(int id)
        {
          return (_context.Portion?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
