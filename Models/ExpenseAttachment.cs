using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class ExpenseAttachment
    {
        public int Id { get; set; }

        [Required]
        public int ExpenseId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Expense? Expense { get; set; }
    }
}
