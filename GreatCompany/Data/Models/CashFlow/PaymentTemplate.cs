using QS.DomainModel.Entity;

namespace GreatCompany.Data.Models;

[Appellative(Gender = GrammaticalGender.Masculine, Nominative = "шаблон платежа", NominativePlural = "шаблоны платежей")]
public class PaymentTemplate : ExpenseDocument {
}
