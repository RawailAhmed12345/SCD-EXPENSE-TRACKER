using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Month == now.Month && b.Year == now.Year)
                .ToListAsync();

            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var expenses = await _context.Expenses
                .Where(e => e.Date >= startOfMonth)
                .ToListAsync();

            var budgetStatuses = budgets.Select(b =>
            {
                var spent = expenses
                    .Where(e => e.CategoryId == b.CategoryId)
                    .Sum(e => e.Amount);

                var percentageUsed = b.Amount > 0 ? (spent / b.Amount) * 100 : 0;

                return new BudgetStatus
                {
                    CategoryName = b.Category?.Name ?? "Unknown",
                    CategoryColor = b.Category?.Color ?? "#64748b",
                    BudgetAmount = b.Amount,
                    SpentAmount = spent,
                    RemainingAmount = b.Amount - spent,
                    PercentageUsed = percentageUsed,
                    IsOverBudget = spent > b.Amount,
                    IsNearLimit = percentageUsed >= b.AlertThreshold && percentageUsed < 100
                };
            }).ToList();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(budgetStatuses);
        }

        // GET: Budgets/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Month,Year,Amount,AlertThreshold")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                // Check if budget already exists for this category and month
                var existing = await _context.Budgets
                    .FirstOrDefaultAsync(b => b.CategoryId == budget.CategoryId && 
                                            b.Month == budget.Month && 
                                            b.Year == budget.Year);

                if (existing != null)
                {
                    ModelState.AddModelError("", "A budget already exists for this category and month.");
                    ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                    return View(budget);
                }

                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets.FindAsync(id);
            if (budget == null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Month,Year,Amount,AlertThreshold")] Budget budget)
        {
            if (id != budget.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Budgets.AnyAsync(e => e.Id == budget.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
