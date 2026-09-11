using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.CashFlow;
using QS.DomainModel.UoW;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

public class ActualIncomeViewModel : IncomeCardViewModelBase<ActualIncome> {
	public ActualIncomeViewModel(
		IEntityUoWBuilder uowBuilder,
		CardDependencies deps,
		// Журнал открывает карточку по шаблону, передавая его номер
		// при обычном создании 0
		int templateId = 0)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<ActualIncome>(this, Entity, UoW, deps.Navigation, deps.Scope);

		PlannedIncomeEntry = builder.ForProperty(x => x.PlannedIncome)
			.UseViewModelJournalAndAutocompleter<PlannedIncomeJournalViewModel>()
			.UseViewModelDialog<PlannedIncomeViewModel>()
			.Finish();

		FillFromTemplate(templateId);
	}

	public IEntityEntryViewModel PlannedIncomeEntry { get; }
}
