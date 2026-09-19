using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "факт расхода", NominativePlural = "факт - расход")]
public class ActualExpense : ExpenseOperation {
	PlannedExpense? plannedExpense;
	[Display(Name = "Ссылка на план")]
	public virtual PlannedExpense? PlannedExpense { get => plannedExpense; set => SetField(ref plannedExpense, value); }
}
