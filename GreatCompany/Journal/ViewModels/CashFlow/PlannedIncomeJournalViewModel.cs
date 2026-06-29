using System.Reactive;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Templates;
using GreatCompany.Navigation;
using GreatCompany.ViewModels.CashFlow;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class PlannedIncomeJournalViewModel : JournalViewModelBase<PlannedIncomeRow> {
	readonly Repository repo;

	public PlannedIncomeJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "План - приход";
		CreateFromTemplateCommand = ReactiveCommand.Create(CreateFromTemplate);
		Reload();
	}

	public ReactiveCommand<Unit, Unit> CreateFromTemplateCommand { get; }

	protected override IEnumerable<PlannedIncomeRow> Load(string search) => repo.PlannedIncomes(search);
	protected override void Create() => OpenCard<PlannedIncomeViewModel>(0);
	protected override void Edit(PlannedIncomeRow row) => OpenCard<PlannedIncomeViewModel>(row.Id);
	protected override void Delete(PlannedIncomeRow row) { repo.Delete<PlannedIncome>(row.Id); Reload(); }

	void CreateFromTemplate() {
		NavigationManager.OpenReferenceSelect<AccrualTemplateJournalViewModel>(this, template => {
			var page = NavigationManager.OpenViewModel<PlannedIncomeViewModel, int>(
				this, 0, OpenPageOptions.IgnoreHash, card => card.ApplyTemplate(template.Id));
			page.ViewModel.Saved += Reload;
		});
	}
}
