using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class RecurringExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecurringExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RecurringExpenses
        public async Task<IActionResult> Index()
        {
            var recurringExpenses = await _context.RecurringExpenses
                .Include(r => r.Category)
                .Where(r => r.IsActive)
                .ToListAsync();

            return View(recurringExpenses);
        }

        // GET: RecurringExpenses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        // POST: RecurringExpenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Description,Amount,Frequency,StartDate,EndDate,DayOfMonth,DayOfWeek")] RecurringExpense recurringExpense)
        {
            if (ModelState.IsValid)
            {
                recurringExpense.IsActive = true;
                _context.Add(recurringExpense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(recurringExpense);
        }

        // GET: RecurringExpenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var recurringExpense = await _context.RecurringExpenses.FindAsync(id);
            if (recurringExpense == null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(recurringExpense);
        }

        // POST: RecurringExpenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Description,Amount,Frequency,StartDate,EndDate,IsActive,DayOfMonth,DayOfWeek")] RecurringExpense recurringExpense)
        {
            if (id != recurringExpense.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recurringExpense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.RecurringExpenses.AnyAsync(e => e.Id == recurringExpense.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(recurringExpense);
        }

        // POST: RecurringExpenses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var recurringExpense = await _context.RecurringExpenses.FindAsync(id);
            if (recurringExpense != null)
            {
                recurringExpense.IsActive = false;
                _context.Update(recurringExpense);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: RecurringExpenses/Generate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(int id)
        {
            var recurringExpense = await _context.RecurringExpenses
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recurringExpense == null || !recurringExpense.IsActive)
                return NotFound();

            // Create a new expense from the recurring template
            var expense = new Expense
            {
                CategoryId = recurringExpense.CategoryId,
                Category = recurringExpense.Category?.Name,
                Description = recurringExpense.Description,
                Amount = recurringExpense.Amount,
                Date = DateTime.Now,
                IsRecurring = true,
                RecurringExpenseId = recurringExpense.Id
            };

            _context.Expenses.Add(expense);

            // Update last generated timestamp
            recurringExpense.LastGenerated = DateTime.Now;
            _context.Update(recurringExpense);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
