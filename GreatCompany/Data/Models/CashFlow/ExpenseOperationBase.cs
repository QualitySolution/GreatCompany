using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;
using QS.Validation;

namespace GreatCompany.Data.Models;

public abstract class ExpenseOperationBase : ExpenseDocumentBase {
	DateTime? date = DateTime.Today;
	[Display(Name = "Дата")]
	[RequiredField]
	[PropertyChangedAlso(nameof(Title))]
	public virtual DateTime? Date { get => date; set => SetField(ref date, value); }

	public override string Title => $"{Purpose} от {Date:dd.MM.yyyy}";
}
