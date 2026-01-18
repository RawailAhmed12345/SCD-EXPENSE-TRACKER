using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(int? categoryId, DateTime? startDate, DateTime? endDate, string? q)
        {
            var query = _context.Expenses
                .Include(e => e.CategoryNavigation)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value.Date);

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(e => e.Date <= endOfDay);
            }

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(e => e.Description != null &&
                                         EF.Functions.Like(e.Description, $"%{q}%"));

            var items = await query
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var now = DateTime.Now;

            var vm = new ExpenseIndexViewModel
            {
                Expenses = items,
                TotalAmount = items.Sum(i => i.Amount),
                MonthAmount = items
                    .Where(i => i.Date.Month == now.Month && i.Date.Year == now.Year)
                    .Sum(i => i.Amount),
                CategoryId = categoryId,
                StartDate = startDate,
                EndDate = endDate,
                Query = q
            };

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(vm);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.CategoryNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        // GET: Expenses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(expense.CategoryId);
                if (category == null) return BadRequest();

                // Optional legacy value
                expense.Category = category.Name;

                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _context.Categories.FindAsync(expense.CategoryId);
                    if (category != null)
                        expense.Category = category.Name;

                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Expenses.AnyAsync(e => e.Id == expense.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.CategoryNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
