using QS.Journal;
using QS.Project.Journal;

namespace GreatCompany.Journal.ViewModels.Templates;

/// <summary>фильтр журналов шаблонов начислений и платежей</summary>
public class TemplateFilterViewModel : JournalFilterViewModelBase<TemplateFilterViewModel> {
	public TemplateFilterViewModel(IJournalViewModel journalViewModel) : base(journalViewModel) {
	}

	bool showDisabled;
	public virtual bool ShowDisabled { get => showDisabled; set => SetField(ref showDisabled, value); }
}
