using QS.Journal;
using QS.Project.Journal;

namespace GreatCompany.Journal.ViewModels.Reference;

public class ProjectFilterViewModel : JournalFilterViewModelBase<ProjectFilterViewModel> {
	public ProjectFilterViewModel(IJournalViewModel journalViewModel) : base(journalViewModel) {
	}

	bool showArchived;
	public virtual bool ShowArchived { get => showArchived; set => SetField(ref showArchived, value); }
}
