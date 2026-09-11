using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.Reference;

public class DivisionViewModel : CardViewModelBase<Division> {
	public DivisionViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<Division>(this, Entity, UoW, deps.Navigation, deps.Scope);

		ParentDivisionEntry = builder.ForProperty(x => x.ParentDivision)
			.UseViewModelJournalAndAutocompleter<DivisionJournalViewModel>()
			.UseViewModelDialog<DivisionViewModel>()
			.Finish();
	}

	public IEntityEntryViewModel ParentDivisionEntry { get; }
}
