using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.CashFlow;
using QS.DomainModel.UoW;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

public class ActualExpenseViewModel : ExpenseCardViewModelBase<ActualExpense> {
	public ActualExpenseViewModel(
		IEntityUoWBuilder uowBuilder,
		CardDependencies deps,
		// Журнал открывает карточку по шаблону, передавая его номер
		// при обычном создании 0
		int templateId = 0)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<ActualExpense>(this, Entity, UoW, deps.Navigation, deps.Scope);

		PlannedExpenseEntry = builder.ForProperty(x => x.PlannedExpense)
			.UseViewModelJournalAndAutocompleter<PlannedExpenseJournalViewModel>()
			.UseViewModelDialog<PlannedExpenseViewModel>()
			.Finish();

		FillFromTemplate(templateId);
	}

	public IEntityEntryViewModel PlannedExpenseEntry { get; }
}
