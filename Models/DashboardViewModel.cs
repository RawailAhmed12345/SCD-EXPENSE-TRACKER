using ExpenseTracker.Models;

namespace ExpenseTracker.Models
{
    public class DashboardViewModel
    {
        // Summary Statistics
        public decimal TotalExpenses { get; set; }
        public decimal MonthExpenses { get; set; }
        public decimal WeekExpenses { get; set; }
        public decimal TodayExpenses { get; set; }
        
        // Comparisons
        public decimal LastMonthExpenses { get; set; }
        public decimal MonthChangePercentage { get; set; }
        
        // Top Categories
        public List<CategorySummary> TopCategories { get; set; } = new();
        
        // Recent Expenses
        public List<Expense> RecentExpenses { get; set; } = new();
        
        // Budget Status
        public List<BudgetStatus> BudgetStatuses { get; set; } = new();
        
        // Monthly Trends (last 6 months)
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class BudgetStatus
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal PercentageUsed { get; set; }
        public bool IsOverBudget { get; set; }
        public bool IsNearLimit { get; set; }
    }

    public class MonthlyTrend
    {
        public string MonthName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }
}
