using GreatCompany.Data.Models;
using QS.DomainModel.UoW;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.CashFlow;

public class PlannedIncomeViewModel : IncomeCardViewModelBase<PlannedIncome> {
	public PlannedIncomeViewModel(
		IEntityUoWBuilder uowBuilder,
		CardDependencies deps,
		// журнал открывает карточку по шаблону и передаёт его номер.
		// при обычном создании 0
		int templateId = 0)
		: base(uowBuilder, deps) {
		FillFromTemplate(templateId);
	}
}
