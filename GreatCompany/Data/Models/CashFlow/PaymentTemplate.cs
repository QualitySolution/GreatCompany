using System.ComponentModel.DataAnnotations;
using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "шаблон платежа", NominativePlural = "шаблоны платежей")]
public class PaymentTemplate : ExpenseDocumentBase {
	// отключённый шаблон не предлагается при создании документа по шаблону
	bool isDisabled;
	[Display(Name = "Отключён")]
	public virtual bool IsDisabled { get => isDisabled; set => SetField(ref isDisabled, value); }
}
