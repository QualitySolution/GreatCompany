using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "план прихода", NominativePlural = "план - приход")]
public class PlannedIncome : PropertyChangedBase, IDomainObject {
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

	int projectId;
	[Display(Name = "Проект")]
	[Range(1, int.MaxValue, ErrorMessage = "Выберите проект")]
	public virtual int ProjectId { get => projectId; set => SetField(ref projectId, value); }

	int incomeArticleId;
	[Display(Name = "Статья дохода")]
	[Range(1, int.MaxValue, ErrorMessage = "Выберите статью дохода")]
	public virtual int IncomeArticleId { get => incomeArticleId; set => SetField(ref incomeArticleId, value); }
}

public class PlannedIncomeRow {
	public int Id { get; set; }
	public DateTime Date { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string AccountName { get; set; } = "";
	public string ProjectName { get; set; } = "";
	public string ArticleName { get; set; } = "";
}
