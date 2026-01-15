using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfToday = now.Date;

            var expenses = await _context.Expenses
                .Include(e => e.CategoryNavigation)
                .ToListAsync();

            // Summary Statistics
            var totalExpenses = expenses.Sum(e => e.Amount);
            var monthExpenses = expenses.Where(e => e.Date >= startOfMonth).Sum(e => e.Amount);
            var weekExpenses = expenses.Where(e => e.Date >= startOfWeek).Sum(e => e.Amount);
            var todayExpenses = expenses.Where(e => e.Date >= startOfToday).Sum(e => e.Amount);

            // Last month comparison
            var lastMonthStart = startOfMonth.AddMonths(-1);
            var lastMonthExpenses = expenses
                .Where(e => e.Date >= lastMonthStart && e.Date < startOfMonth)
                .Sum(e => e.Amount);

            var monthChangePercentage = lastMonthExpenses > 0
                ? ((monthExpenses - lastMonthExpenses) / lastMonthExpenses) * 100
                : 0;

            // Top Categories (current month)
            var monthExpensesList = expenses.Where(e => e.Date >= startOfMonth).ToList();
            var topCategories = monthExpensesList
                .GroupBy(e => new
                {
                    Name = e.CategoryNavigation?.Name ?? e.Category ?? "Uncategorized",
                    Icon = e.CategoryNavigation?.Icon ?? "fa-circle",
                    Color = e.CategoryNavigation?.Color ?? "#64748b"
                })
                .Select(g => new CategorySummary
                {
                    CategoryName = g.Key.Name,
                    CategoryIcon = g.Key.Icon,
                    CategoryColor = g.Key.Color,
                    TotalAmount = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count(),
                    Percentage = monthExpenses > 0 ? (g.Sum(e => e.Amount) / monthExpenses) * 100 : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(5)
                .ToList();

            // Recent Expenses
            var recentExpenses = expenses
                .OrderByDescending(e => e.Date)
                .Take(10)
                .ToList();

            // Budget Status
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.Month == now.Month && b.Year == now.Year)
                .ToListAsync();

            var budgetStatuses = budgets.Select(b =>
            {
                var spent = expenses
                    .Where(e => e.CategoryId == b.CategoryId && e.Date >= startOfMonth)
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

            // Monthly Trends (last 6 months)
            var monthlyTrends = new List<MonthlyTrend>();
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var monthTotal = expenses
                    .Where(e => e.Date >= monthStart && e.Date < monthEnd)
                    .Sum(e => e.Amount);

                monthlyTrends.Add(new MonthlyTrend
                {
                    MonthName = month.ToString("MMM yyyy"),
                    Month = month.Month,
                    Year = month.Year,
                    Amount = monthTotal
                });
            }

            var viewModel = new DashboardViewModel
            {
                TotalExpenses = totalExpenses,
                MonthExpenses = monthExpenses,
                WeekExpenses = weekExpenses,
                TodayExpenses = todayExpenses,
                LastMonthExpenses = lastMonthExpenses,
                MonthChangePercentage = monthChangePercentage,
                TopCategories = topCategories,
                RecentExpenses = recentExpenses,
                BudgetStatuses = budgetStatuses,
                MonthlyTrends = monthlyTrends
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryBreakdown()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var expenses = await _context.Expenses
                .Include(e => e.CategoryNavigation)
                .Where(e => e.Date >= startOfMonth)
                .ToListAsync();

            var breakdown = expenses
                .GroupBy(e => new
                {
                    Name = e.CategoryNavigation?.Name ?? e.Category ?? "Uncategorized",
                    Color = e.CategoryNavigation?.Color ?? "#64748b"
                })
                .Select(g => new
                {
                    label = g.Key.Name,
                    value = g.Sum(e => e.Amount),
                    color = g.Key.Color
                })
                .OrderByDescending(x => x.value)
                .ToList();

            return Json(breakdown);
        }
    }
}
