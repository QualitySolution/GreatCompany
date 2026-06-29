using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class AccountJournalViewModel : ReferenceJournalViewModelBase<AccountRow> {
	readonly Repository repo;

	public AccountJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "Счета";
		Reload();
	}

	protected override IEnumerable<AccountRow> Load(string search) => repo.Accounts(search);
	protected override void Create() => OpenCard<AccountViewModel>(0);
	protected override void Edit(AccountRow row) => OpenCard<AccountViewModel>(row.Id);
	protected override void Delete(AccountRow row) { repo.Delete<Account>(row.Id); Reload(); }
}
