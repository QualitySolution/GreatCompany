using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

// Шаблон начисления — приход без даты.
[Appellative(Nominative = "шаблон начисления", NominativePlural = "шаблоны начислений")]
public class AccrualTemplate : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string purpose = "";
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	decimal amount;
	public virtual decimal Amount { get => amount; set => SetField(ref amount, value); }

	decimal vatAmount;
	public virtual decimal VatAmount { get => vatAmount; set => SetField(ref vatAmount, value); }

	int accountId;
	public virtual int AccountId { get => accountId; set => SetField(ref accountId, value); }

	int projectId;
	public virtual int ProjectId { get => projectId; set => SetField(ref projectId, value); }

	int incomeArticleId;
	public virtual int IncomeArticleId { get => incomeArticleId; set => SetField(ref incomeArticleId, value); }
}

public class AccrualTemplateRow : IReferenceRow {
	public int Id { get; set; }
	public string Purpose { get; set; } = "";
	public string Name => Purpose;
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string AccountName { get; set; } = "";
	public string ProjectName { get; set; } = "";
	public string ArticleName { get; set; } = "";
}
