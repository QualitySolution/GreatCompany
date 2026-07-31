using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Dialog;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class AccountJournalViewModel : ReferenceJournalViewModelBase<AccountRow> {
	readonly Repository repo;

	public AccountJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Счета";
		Reload();
	}

	protected override IEnumerable<AccountRow> Load(string search) => repo.Accounts(search);
	protected override void Create() => OpenCard<AccountViewModel>(0);
	protected override void Edit(AccountRow row) => OpenCard<AccountViewModel>(row.Id);
	protected override void Delete(AccountRow row) { if(repo.Delete<Account>(row.Id)) Reload(); else NotifyDeleteBlocked(); }
}
