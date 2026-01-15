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
        // Supports optional filtering via query string: category, startDate, endDate, q (search)
        public async Task<IActionResult> Index(string? category, DateTime? startDate, DateTime? endDate, string? q)
        {
            var query = _context.Expenses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category == category);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value.Date);

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(e => e.Date <= endOfDay);
            }

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(e => e.Description != null && EF.Functions.Like(e.Description, $"%{q}%"));

            var items = await query.OrderByDescending(e => e.Date).ToListAsync();

            var total = items.Sum(i => i.Amount);
            var now = DateTime.Now;
            var monthTotal = items.Where(i => i.Date.Year == now.Year && i.Date.Month == now.Month).Sum(i => i.Amount);

            var vm = new ExpenseIndexViewModel
            {
                Expenses = items,
                TotalAmount = total,
                MonthAmount = monthTotal,
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                Query = q
            };

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(vm);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var expense = await _context.Expenses.FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create() => View();

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Category,Description,Amount")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Category,Description,Amount")] Expense expense)
        {
            if (id != expense.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var expense = await _context.Expenses.FirstOrDefaultAsync(m => m.Id == id);
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