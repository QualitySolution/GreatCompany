using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "факт расхода", NominativePlural = "факт - расход")]
public class ActualExpense : PropertyChangedBase, IDomainObject, IValidatableObject {
	public virtual int Id { get; set; }

	DateTime date = DateTime.Today;
	[Display(Name = "Дата")]
	public virtual DateTime Date { get => date; set => SetField(ref date, value); }

	string purpose = "";
	[Display(Name = "Назначение")]
	[Required(ErrorMessage = "Заполните назначение")]
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	decimal amount;
	[Display(Name = "Сумма")]
	public virtual decimal Amount { get => amount; set => SetField(ref amount, value); }

	decimal vatAmount;
	[Display(Name = "Сумма НДС")]
	public virtual decimal VatAmount { get => vatAmount; set => SetField(ref vatAmount, value); }

	int accountId;
	[Display(Name = "Счёт")]
	[Range(1, int.MaxValue, ErrorMessage = "Выберите счёт")]
	public virtual int AccountId { get => accountId; set => SetField(ref accountId, value); }

	int? divisionId;
	[Display(Name = "подразделение")]
	public virtual int? DivisionId { get => divisionId; set => SetField(ref divisionId, value); }

	int? projectId;
	[Display(Name = "Проект")]
	public virtual int? ProjectId { get => projectId; set => SetField(ref projectId, value); }

	int expenseArticleId;
	[Display(Name = "Статья расхода")]
	[Range(1, int.MaxValue, ErrorMessage = "Выберите статью расхода")]
	public virtual int ExpenseArticleId { get => expenseArticleId; set => SetField(ref expenseArticleId, value); }

	int? plannedExpenseId;
	[Display(Name = "Ссылка на план")]
	public virtual int? PlannedExpenseId { get => plannedExpenseId; set => SetField(ref plannedExpenseId, value); }

	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
		if(DivisionId == null && ProjectId == null)
			yield return new ValidationResult("Укажите подразделение или проект", new[] { nameof(DivisionId), nameof(ProjectId) });
	}
}

public class ActualExpenseRow {
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
