using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

public abstract class ExpenseDocumentBase : PropertyChangedBase, IDomainObject {
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

	Division division = null!;
	[Display(Name = "Подразделение")]
	[RequiredField]
	public virtual Division Division { get => division; set => SetField(ref division, value); }

	Project? project;
	[Display(Name = "Проект")]
	public virtual Project? Project {
		get => project;
		// при выборе проекта подставляем его подразделение, поменять его после можно
		set {
			if(SetField(ref project, value) && value != null)
				Division = value.Division;
		}
	}

	ExpenseArticle expenseArticle = null!;
	[Display(Name = "Статья расхода")]
	[RequiredField]
	public virtual ExpenseArticle ExpenseArticle { get => expenseArticle; set => SetField(ref expenseArticle, value); }

	public virtual void FillFrom(ExpenseDocumentBase source) {
		Purpose = source.Purpose;
		Cost = source.Cost;
		Vat = source.Vat;
		Account = source.Account;
		ExpenseArticle = source.ExpenseArticle;
		Project = source.Project;
		Division = source.Division;
	}
}
