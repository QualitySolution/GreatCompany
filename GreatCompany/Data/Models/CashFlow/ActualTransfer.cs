using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

/// <summary>перевод между своими счетами. Не доход и не расход, поэтому без проекта, статьи и НДС</summary>
[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "перевод", NominativePlural = "переводы")]
public class ActualTransfer : PropertyChangedBase, IDomainObject, IValidatableObject {
	public virtual int Id { get; set; }

	DateTime? date = DateTime.Today;
	[Display(Name = "Дата")]
	[RequiredField]
	[PropertyChangedAlso(nameof(Title))]
	public virtual DateTime? Date { get => date; set => SetField(ref date, value); }

	string purpose = "";
	[Display(Name = "Назначение")]
	[RequiredField]
	[PropertyChangedAlso(nameof(Title))]
	public virtual string Purpose { get => purpose; set => SetField(ref purpose, value); }

	public virtual string Title => $"{Purpose} от {Date:dd.MM.yyyy}";

	decimal? cost = 0;
	[Display(Name = "Сумма")]
	[RequiredField]
	public virtual decimal? Cost { get => cost; set => SetField(ref cost, value); }

	Account fromAccount = null!;
	[Display(Name = "Со счёта")]
	[RequiredField]
	public virtual Account FromAccount { get => fromAccount; set => SetField(ref fromAccount, value); }

	Account toAccount = null!;
	[Display(Name = "На счёт")]
	[RequiredField]
	public virtual Account ToAccount { get => toAccount; set => SetField(ref toAccount, value); }

	bool isLoan;
	/// <summary>
	/// деньги придётся вернуть, например между ООО и ИП. Возврат вводится переводом в обратную сторону тоже с этим флагом,
	/// долг - баланс таких переводов между сторонами
	/// </summary>
	[Display(Name = "В долг")]
	public virtual bool IsLoan { get => isLoan; set => SetField(ref isLoan, value); }

	public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
		if(FromAccount != null && ToAccount != null && FromAccount.Id == ToAccount.Id)
			yield return new ValidationResult("счета перевода должны отличаться", new[] { nameof(ToAccount) });

		if(Cost <= 0)
			yield return new ValidationResult("сумма перевода должна быть больше нуля", new[] { nameof(Cost) });
	}
}
