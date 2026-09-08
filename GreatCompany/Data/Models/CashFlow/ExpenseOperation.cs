using System.ComponentModel.DataAnnotations;

namespace GreatCompany.Data.Models;

public abstract class ExpenseOperation : ExpenseDocument {
	DateTime date = DateTime.Today;
	[Display(Name = "Дата")]
	public virtual DateTime Date { get => date; set => SetField(ref date, value); }

	public override string Title => $"{Purpose} от {Date:dd.MM.yyyy}";
}
