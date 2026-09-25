using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

public abstract class IncomeDocument : PropertyChangedBase, IDomainObject {
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

	Project project = null!;
	[Display(Name = "Проект")]
	[RequiredField]
	public virtual Project Project { get => project; set => SetField(ref project, value); }

	IncomeArticle incomeArticle = null!;
	[Display(Name = "Статья дохода")]
	[RequiredField]
	public virtual IncomeArticle IncomeArticle { get => incomeArticle; set => SetField(ref incomeArticle, value); }

	public virtual void FillFrom(IncomeDocument source) {
		Purpose = source.Purpose;
		Amount = source.Amount;
		VatAmount = source.VatAmount;
		Account = source.Account;
		Project = source.Project;
		IncomeArticle = source.IncomeArticle;
	}
}
