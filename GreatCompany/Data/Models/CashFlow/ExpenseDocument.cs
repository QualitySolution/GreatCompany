using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

public abstract class ExpenseDocument : PropertyChangedBase, IDomainObject, IValidatableObject {
	public virtual int Id { get; set; }

	string purpose = "";
	[Display(Name = "Назначение")]
	[RequiredField]
	public virtual string Purpose {
		get => purpose;
		set {
			if(SetField(ref purpose, value))
				OnPropertyChanged(nameof(Title));
		}
	}

	public virtual string Title => Purpose;

	decimal amount;
	[Display(Name = "Сумма")]
	public virtual decimal Amount { get => amount; set => SetField(ref amount, value); }

	decimal vatAmount;
	[Display(Name = "Сумма НДС")]
	public virtual decimal VatAmount { get => vatAmount; set => SetField(ref vatAmount, value); }

	Account account = null!;
	[Display(Name = "Счёт")]
	[RequiredField]
	public virtual Account Account { get => account; set => SetField(ref account, value); }

	Division? division;
	[Display(Name = "Подразделение")]
	public virtual Division? Division { get => division; set => SetField(ref division, value); }

	Project? project;
	[Display(Name = "Проект")]
	public virtual Project? Project {
		get => project;
		// Расход по проекту всегда относится к подразделению этого проекта
		set {
			if(SetField(ref project, value) && value != null)
				Division = value.Division;
		}
	}

	ExpenseArticle expenseArticle = null!;
	[Display(Name = "Статья расхода")]
	[RequiredField]
	public virtual ExpenseArticle ExpenseArticle { get => expenseArticle; set => SetField(ref expenseArticle, value); }

	// Подразделение обязательно или берётся из проекта
	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
		if(Division == null && Project == null)
			yield return new ValidationResult("Укажите подразделение или проект", new[] { nameof(Division), nameof(Project) });
	}

	public virtual void FillFrom(ExpenseDocument source) {
		Purpose = source.Purpose;
		Amount = source.Amount;
		VatAmount = source.VatAmount;
		Account = source.Account;
		ExpenseArticle = source.ExpenseArticle;
		Project = source.Project;
		if(source.Project == null)
			Division = source.Division;
	}
}
