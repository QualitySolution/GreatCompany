using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "факт прихода", NominativePlural = "факт - приход")]
public class ActualIncome : IncomeOperation {
	PlannedIncome? plannedIncome;
	[Display(Name = "Ссылка на план")]
	public virtual PlannedIncome? PlannedIncome { get => plannedIncome; set => SetField(ref plannedIncome, value); }
}
