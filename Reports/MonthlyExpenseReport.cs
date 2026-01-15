using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using ExpenseTracker.Models; // Expense class

namespace SCD_EXPENSE_TRACKER.Reports
{
	public class MonthlyExpenseReport : IDocument
	{
		private readonly List<Expense> _expenses;
		private readonly string _title;

		public MonthlyExpenseReport(List<Expense> expenses, string title)
		{
			_expenses = expenses;
			_title = title;
		}

		public void Compose(IDocumentContainer container)
		{
			container.Page(page =>
			{
				page.Margin(20);

				// Header
				page.Header()
					.Text(_title)
					.SemiBold()
					.FontSize(20)
					.FontColor(Colors.Blue.Medium);

				// Content Table
				page.Content().Table(table =>
				{
					table.ColumnsDefinition(columns =>
					{
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
					});

					// Table Header
					table.Header(header =>
					{
						header.Cell().Text("Date");
						header.Cell().Text("Category");
						header.Cell().Text("Description");
						header.Cell().Text("Amount");
					});

					// Table Rows
					foreach (var expense in _expenses)
					{
						table.Cell().Text(expense.Date.ToString("yyyy-MM-dd"));
						table.Cell().Text(expense.Category ?? "N/A");
						table.Cell().Text(expense.Description ?? "-");
						table.Cell().Text(expense.Amount.ToString("C"));
					}
				});
			});
		}

		public byte[] GeneratePdf()
		{
			using var ms = new MemoryStream();
			this.GeneratePdf(ms); // IDocument extension
			return ms.ToArray();
		}
	}
}
