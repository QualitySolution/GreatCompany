using System.Reactive;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Templates;
using GreatCompany.Navigation;
using GreatCompany.ViewModels.CashFlow;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class PlannedExpenseJournalViewModel : JournalViewModelBase<PlannedExpenseRow> {
	readonly Repository repo;

	public PlannedExpenseJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "План - расход";
		CreateFromTemplateCommand = ReactiveCommand.Create(CreateFromTemplate);
		Reload();
	}

	public ReactiveCommand<Unit, Unit> CreateFromTemplateCommand { get; }

	protected override IEnumerable<PlannedExpenseRow> Load(string search) => repo.PlannedExpenses(search);
	protected override void Create() => OpenCard<PlannedExpenseViewModel>(0);
	protected override void Edit(PlannedExpenseRow row) => OpenCard<PlannedExpenseViewModel>(row.Id);
	protected override void Delete(PlannedExpenseRow row) { repo.Delete<PlannedExpense>(row.Id); Reload(); }

	void CreateFromTemplate() {
		NavigationManager.OpenReferenceSelect<PaymentTemplateJournalViewModel>(this, template => {
			var page = NavigationManager.OpenViewModel<PlannedExpenseViewModel, int>(
				this, 0, OpenPageOptions.IgnoreHash, card => card.ApplyTemplate(template.Id));
			page.ViewModel.Saved += Reload;
		});
	}
}
