using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? Category { get; set; } // Legacy name

        // Nullable to safely handle nulls in database
        public int? CategoryId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Tags { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        public bool IsRecurring { get; set; } = false;

        public int? RecurringExpenseId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        // Navigation
        public Category? CategoryNavigation { get; set; }

        public ICollection<ExpenseAttachment> Attachments { get; set; } = new List<ExpenseAttachment>();
    }
}
