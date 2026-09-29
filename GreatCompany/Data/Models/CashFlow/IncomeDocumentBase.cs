using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

public abstract class IncomeDocumentBase : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string purpose = "";
	[Display(Name = "Назначение")]
	[RequiredField]
	[PropertyChangedAlso(nameof(Title))]
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	public virtual string Title => Purpose;

	decimal? cost = 0;
	[Display(Name = "Сумма")]
	[RequiredField]
	public virtual decimal? Cost { get => cost; set => SetField(ref cost, value); }

	decimal? vat = 0;
	[Display(Name = "Сумма НДС")]
	[RequiredField]
	public virtual decimal? Vat { get => vat; set => SetField(ref vat, value); }

	Account account = null!;
	[Display(Name = "Счёт")]
	[RequiredField]
	public virtual Account Account { get => account; set => SetField(ref account, value); }

	Project project = null!;
	[Display(Name = "Проект")]
	[RequiredField]
	public virtual Project Project { get => project; set => SetField(ref project, value); }

	IncomeArticle incomeArticle = null!;
	[Display(Name = "Статья дохода")]
	[RequiredField]
	public virtual IncomeArticle IncomeArticle { get => incomeArticle; set => SetField(ref incomeArticle, value); }

	public virtual void FillFrom(IncomeDocumentBase source) {
		Purpose = source.Purpose;
		Cost = source.Cost;
		Vat = source.Vat;
		Account = source.Account;
		Project = source.Project;
		IncomeArticle = source.IncomeArticle;
	}
}
