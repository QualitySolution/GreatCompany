using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Reference;
using GreatCompany.ViewModels.Reference;
using QS.DomainModel.UoW;
using QS.Project.Domain;
using QS.ViewModels.Control.EEVM;

namespace GreatCompany.ViewModels.CashFlow;

public class ActualTransferViewModel : CardViewModelBase<ActualTransfer> {
	public ActualTransferViewModel(IEntityUoWBuilder uowBuilder, CardDependencies deps)
		: base(uowBuilder, deps) {
		var builder = new CommonEEVMBuilderFactory<ActualTransfer>(this, Entity, UoW, deps.Navigation, deps.Scope);

		FromAccountEntry = builder.ForProperty(x => x.FromAccount)
			.UseViewModelJournalAndAutocompleter<AccountJournalViewModel>()
			.UseViewModelDialog<AccountViewModel>()
			.Finish();

		ToAccountEntry = builder.ForProperty(x => x.ToAccount)
			.UseViewModelJournalAndAutocompleter<AccountJournalViewModel>()
			.UseViewModelDialog<AccountViewModel>()
			.Finish();
	}

	public IEntityEntryViewModel FromAccountEntry { get; }
	public IEntityEntryViewModel ToAccountEntry { get; }
}
