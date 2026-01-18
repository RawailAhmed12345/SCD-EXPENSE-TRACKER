using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Icon { get; set; } = "fa-circle";

        [StringLength(7)]
        public string Color { get; set; } = "#6366f1";

        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<RecurringExpense> RecurringExpenses { get; set; } = new List<RecurringExpense>();
    }
}
