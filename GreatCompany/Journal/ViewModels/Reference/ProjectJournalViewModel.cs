using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class ProjectJournalViewModel : ReferenceJournalViewModelBase<ProjectRow> {
	readonly Repository repo;

	public ProjectJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "Проекты";
		Reload();
	}

	protected override IEnumerable<ProjectRow> Load(string search) => repo.Projects(search);
	protected override void Create() => OpenCard<ProjectViewModel>(0);
	protected override void Edit(ProjectRow row) => OpenCard<ProjectViewModel>(row.Id);
	protected override void Delete(ProjectRow row) { repo.Delete<Project>(row.Id); Reload(); }
}
