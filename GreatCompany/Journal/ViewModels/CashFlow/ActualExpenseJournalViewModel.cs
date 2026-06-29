using System.Reactive;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Templates;
using GreatCompany.Navigation;
using GreatCompany.ViewModels.CashFlow;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class ActualExpenseJournalViewModel : JournalViewModelBase<ActualExpenseRow> {
	readonly Repository repo;

	public ActualExpenseJournalViewModel(Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Title = "Факт - расход";
		CreateFromTemplateCommand = ReactiveCommand.Create(CreateFromTemplate);
		Reload();
	}

	public ReactiveCommand<Unit, Unit> CreateFromTemplateCommand { get; }

	protected override IEnumerable<ActualExpenseRow> Load(string search) => repo.ActualExpenses(search);
	protected override void Create() => OpenCard<ActualExpenseViewModel>(0);
	protected override void Edit(ActualExpenseRow row) => OpenCard<ActualExpenseViewModel>(row.Id);
	protected override void Delete(ActualExpenseRow row) { repo.Delete<ActualExpense>(row.Id); Reload(); }

	void CreateFromTemplate() {
		NavigationManager.OpenReferenceSelect<PaymentTemplateJournalViewModel>(this, template => {
			var page = NavigationManager.OpenViewModel<ActualExpenseViewModel, int>(
				this, 0, OpenPageOptions.IgnoreHash, card => card.ApplyTemplate(template.Id));
			page.ViewModel.Saved += Reload;
		});
	}
}
