using GreatCompany.Data.Models;
using GreatCompany.ViewModels.CashFlow;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.Templates;

public class AccrualTemplateViewModel : IncomeCardViewModelBase<AccrualTemplate> {
	public AccrualTemplateViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
	}
}
