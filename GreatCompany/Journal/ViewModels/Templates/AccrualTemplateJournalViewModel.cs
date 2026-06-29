using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Templates;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Templates;

public class AccrualTemplateJournalViewModel : ReferenceJournalViewModelBase<AccrualTemplateRow> {
	readonly Repository repo;

	public AccrualTemplateJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "Шаблоны начислений";
		Reload();
	}

	protected override IEnumerable<AccrualTemplateRow> Load(string search) => repo.AccrualTemplates(search);
	protected override void Create() => OpenCard<AccrualTemplateViewModel>(0);
	protected override void Edit(AccrualTemplateRow row) => OpenCard<AccrualTemplateViewModel>(row.Id);
	protected override void Delete(AccrualTemplateRow row) { repo.Delete<AccrualTemplate>(row.Id); Reload(); }
}
