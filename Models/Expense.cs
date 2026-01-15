using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Keep for backward compatibility, but prefer CategoryId
        [StringLength(100)]
        public string? Category { get; set; }

        // New foreign key to Category table
        public int? CategoryId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        // New fields
        [StringLength(500)]
        public string? Tags { get; set; } // Comma-separated tags

        [StringLength(2000)]
        public string? Notes { get; set; }

        public bool IsRecurring { get; set; } = false;

        public int? RecurringExpenseId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        // Navigation properties
        public Models.Category? CategoryNavigation { get; set; }
        public ICollection<ExpenseAttachment> Attachments { get; set; } = new List<ExpenseAttachment>();
    }
}