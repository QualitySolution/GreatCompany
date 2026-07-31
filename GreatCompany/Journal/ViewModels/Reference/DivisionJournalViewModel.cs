using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Dialog;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class DivisionJournalViewModel : ReferenceJournalViewModelBase<DivisionRow> {
	readonly Repository repo;

	public DivisionJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Подразделения";
		Reload();
	}

	protected override IEnumerable<DivisionRow> Load(string search) => repo.Divisions(search);
	protected override void Create() => OpenCard<DivisionViewModel>(0);
	protected override void Edit(DivisionRow row) => OpenCard<DivisionViewModel>(row.Id);
	protected override void Delete(DivisionRow row) { if(repo.Delete<Division>(row.Id)) Reload(); else NotifyDeleteBlocked(); }
}
