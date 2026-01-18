using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int Month { get; set; } // 1-12

        [Required]
        public int Year { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Range(0, 100)]
        public int AlertThreshold { get; set; } = 80;

        public Category? Category { get; set; }
    }
}
