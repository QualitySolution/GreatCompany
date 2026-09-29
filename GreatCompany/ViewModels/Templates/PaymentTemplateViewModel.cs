using GreatCompany.Data.Models;
using GreatCompany.ViewModels.CashFlow;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.Templates;

public class PaymentTemplateViewModel : ExpenseCardViewModelBase<PaymentTemplate> {
	public PaymentTemplateViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
	}
}
