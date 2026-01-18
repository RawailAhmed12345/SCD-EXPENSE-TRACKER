using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            // Get all active categories
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            // Count expenses per category safely (skip null CategoryId)
            var categoryCounts = await _context.Expenses
                .Where(e => e.CategoryId.HasValue)
                .GroupBy(e => e.CategoryId.Value) // Safe cast to int
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            ViewBag.CategoryCounts = categoryCounts;

            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create() => View();

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.IsDefault = false;
                category.IsActive = true;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null || category.IsDefault)
                return RedirectToAction(nameof(Index));

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id || category.IsDefault)
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null && !category.IsDefault)
            {
                category.IsActive = false;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/GetCategories (JSON API)
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, c.Name, c.Icon, c.Color })
                .ToListAsync();

            return Json(categories);
        }
    }
}
