using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.Reference;

public class ProjectViewModel : CardViewModelBase<Project> {
	public ProjectViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<Project>(this, Entity, UoW, deps.Navigation, deps.Scope);

		DivisionEntry = builder.ForProperty(x => x.Division)
			.UseViewModelJournalAndAutocompleter<DivisionJournalViewModel>()
			.UseViewModelDialog<DivisionViewModel>()
			.Finish();
	}

	public IEntityEntryViewModel DivisionEntry { get; }
}
