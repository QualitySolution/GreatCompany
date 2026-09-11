using GreatCompany.Data.Models;
using QS.DomainModel.UoW;
using QS.Project.Domain;

namespace GreatCompany.ViewModels.CashFlow;

public class PlannedExpenseViewModel : ExpenseCardViewModelBase<PlannedExpense> {
	public PlannedExpenseViewModel(
		IEntityUoWBuilder uowBuilder,
		CardDependencies deps,
		// Журнал открывает карточку по шаблону, передавая его номер
		// при обычном создании 0
		int templateId = 0)
		: base(uowBuilder, deps) {
		FillFromTemplate(templateId);
	}
}
