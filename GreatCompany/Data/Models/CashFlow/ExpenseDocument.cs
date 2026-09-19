using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

public abstract class ExpenseDocument : PropertyChangedBase, IDomainObject {
	public virtual int Id { get; set; }

	string purpose = "";
	[Display(Name = "Назначение")]
	[RequiredField]
	[PropertyChangedAlso(nameof(Title))]
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	public virtual string Title => Purpose;

	decimal? amount = 0;
	[Display(Name = "Сумма")]
	[RequiredField]
	public virtual decimal? Amount { get => amount; set => SetField(ref amount, value); }

	decimal? vatAmount = 0;
	[Display(Name = "Сумма НДС")]
	[RequiredField]
	public virtual decimal? VatAmount { get => vatAmount; set => SetField(ref vatAmount, value); }

	Account account = null!;
	[Display(Name = "Счёт")]
	[RequiredField]
	public virtual Account Account { get => account; set => SetField(ref account, value); }

	Division? division;
	[Display(Name = "Подразделение")]
	[RequiredField]
	public virtual Division? Division { get => division; set => SetField(ref division, value); }

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

	public virtual void FillFrom(ExpenseDocument source) {
		Purpose = source.Purpose;
		Amount = source.Amount;
		VatAmount = source.VatAmount;
		Account = source.Account;
		ExpenseArticle = source.ExpenseArticle;
		Project = source.Project;
		Division = source.Division;
	}
}
