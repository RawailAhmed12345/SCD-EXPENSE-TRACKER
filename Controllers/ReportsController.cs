using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Data;
using ExpenseTracker.Models;        // For Expense
using SCD_EXPENSE_TRACKER.Reports; // For MonthlyExpenseReport
using System.Linq;

namespace SCD_EXPENSE_TRACKER.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Reports/Monthly?month=1&year=2026
        public IActionResult Monthly(int month, int year)
        {
            var expenses = _context.Expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .ToList();

            // Generate PDF
            var document = new MonthlyExpenseReport(expenses, $"Expense Report - {month}/{year}");
            var pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"MonthlyReport_{month}_{year}.pdf");
        }
    }
}
