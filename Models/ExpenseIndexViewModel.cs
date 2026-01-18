using System;
using System.Collections.Generic;
using ExpenseTracker.Models;

namespace ExpenseTracker.Models
{
    public class ExpenseIndexViewModel
    {
        public List<Expense> Expenses { get; set; } = new();

        public decimal TotalAmount { get; set; }
        public decimal MonthAmount { get; set; }

        // 🔴 OLD (string-based)
        // public string? Category { get; set; }

        // ✅ NEW (foreign key based)
        public int? CategoryId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Query { get; set; }
    }
}
