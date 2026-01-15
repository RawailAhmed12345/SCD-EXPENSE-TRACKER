using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public enum RecurringFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public class RecurringExpense
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        public RecurringFrequency Frequency { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime? LastGenerated { get; set; }

        public int DayOfMonth { get; set; } = 1; // For monthly: which day (1-31)
        public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday; // For weekly

        // Navigation property
        public Category? Category { get; set; }
    }
}
