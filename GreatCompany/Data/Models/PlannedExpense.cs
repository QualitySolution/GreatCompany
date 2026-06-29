using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Nominative = "план расхода", NominativePlural = "план - расход")]
public class PlannedExpense : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	DateTime date = DateTime.Today;
	public virtual DateTime Date { get => date; set => SetField(ref date, value); }

	string purpose = "";
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	decimal amount;
	public virtual decimal Amount { get => amount; set => SetField(ref amount, value); }

	decimal vatAmount;
	public virtual decimal VatAmount { get => vatAmount; set => SetField(ref vatAmount, value); }

	int accountId;
	public virtual int AccountId { get => accountId; set => SetField(ref accountId, value); }

	int? divisionId;
	public virtual int? DivisionId { get => divisionId; set => SetField(ref divisionId, value); }

	int? projectId;
	public virtual int? ProjectId { get => projectId; set => SetField(ref projectId, value); }

	int expenseArticleId;
	public virtual int ExpenseArticleId { get => expenseArticleId; set => SetField(ref expenseArticleId, value); }
}

public class PlannedExpenseRow {
	public int Id { get; set; }
	public DateTime Date { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string AccountName { get; set; } = "";
	public string? DivisionName { get; set; }
	public string? ProjectName { get; set; }
	public string ArticleName { get; set; } = "";
}
