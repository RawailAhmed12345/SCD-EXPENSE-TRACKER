using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;
        public DbSet<RecurringExpense> RecurringExpenses { get; set; } = null!;
        public DbSet<ExpenseAttachment> ExpenseAttachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision fix
            modelBuilder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RecurringExpense>()
                .Property(r => r.Amount)
                .HasColumnType("decimal(18,2)");

            // Relationships
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.CategoryNavigation)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecurringExpense>()
                .HasOne(r => r.Category)
                .WithMany(c => c.RecurringExpenses)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExpenseAttachment>()
                .HasOne(a => a.Expense)
                .WithMany(e => e.Attachments)
                .HasForeignKey(a => a.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed default categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food & Dining", Icon = "fa-utensils", Color = "#ef4444", IsDefault = true },
                new Category { Id = 2, Name = "Transportation", Icon = "fa-car", Color = "#f59e0b", IsDefault = true },
                new Category { Id = 3, Name = "Shopping", Icon = "fa-shopping-bag", Color = "#ec4899", IsDefault = true },
                new Category { Id = 4, Name = "Entertainment", Icon = "fa-film", Color = "#8b5cf6", IsDefault = true },
                new Category { Id = 5, Name = "Bills & Utilities", Icon = "fa-file-invoice-dollar", Color = "#3b82f6", IsDefault = true },
                new Category { Id = 6, Name = "Healthcare", Icon = "fa-heartbeat", Color = "#10b981", IsDefault = true },
                new Category { Id = 7, Name = "Education", Icon = "fa-graduation-cap", Color = "#06b6d4", IsDefault = true },
                new Category { Id = 8, Name = "Travel", Icon = "fa-plane", Color = "#6366f1", IsDefault = true },
                new Category { Id = 9, Name = "Housing", Icon = "fa-home", Color = "#14b8a6", IsDefault = true },
                new Category { Id = 10, Name = "Personal Care", Icon = "fa-spa", Color = "#a855f7", IsDefault = true },
                new Category { Id = 11, Name = "Gifts & Donations", Icon = "fa-gift", Color = "#f43f5e", IsDefault = true },
                new Category { Id = 12, Name = "Other", Icon = "fa-ellipsis-h", Color = "#64748b", IsDefault = true }
            );
        }
    }
}
