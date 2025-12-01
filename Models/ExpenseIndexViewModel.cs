using System;
using System.Collections.Generic;

namespace ExpenseTracker.Models
{
    public class ExpenseIndexViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; } = new List<Expense>();
        public decimal TotalAmount { get; set; }
        public decimal MonthAmount { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Query { get; set; }
    }
}