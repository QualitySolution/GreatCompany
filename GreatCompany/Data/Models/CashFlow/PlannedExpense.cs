using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "план расхода", NominativePlural = "план - расход")]
public class PlannedExpense : ExpenseOperationBase {
	PaymentTemplate? paymentTemplate;
	/// <summary>шаблон, по которому создан план. По нему повторное заполнение месяца не создаёт дубль</summary>
	[Display(Name = "Шаблон платежа")]
	public virtual PaymentTemplate? PaymentTemplate { get => paymentTemplate; set => SetField(ref paymentTemplate, value); }
}
