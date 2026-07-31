using System.Reactive;
using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Templates;
using GreatCompany.Navigation;
using GreatCompany.ViewModels.CashFlow;
using QS.Dialog;
using QS.Navigation;
using ReactiveUI;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class ActualExpenseJournalViewModel : JournalViewModelBase<ActualExpenseRow> {
	readonly Repository repo;

	public ActualExpenseJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Факт - расход";
		CreateFromTemplateCommand = ReactiveCommand.Create(CreateFromTemplate);
		Reload();
	}

	public ReactiveCommand<Unit, Unit> CreateFromTemplateCommand { get; }

	protected override IEnumerable<ActualExpenseRow> Load(string search) => repo.ActualExpenses(search);
	protected override void Create() => OpenCard<ActualExpenseViewModel>(0);
	protected override void Edit(ActualExpenseRow row) => OpenCard<ActualExpenseViewModel>(row.Id);
	protected override void Delete(ActualExpenseRow row) { if(repo.Delete<ActualExpense>(row.Id)) Reload(); else NotifyDeleteBlocked(); }

	void CreateFromTemplate() {
		NavigationManager.OpenReferenceSelect<PaymentTemplateJournalViewModel>(this, template => {
			var page = NavigationManager.OpenViewModel<ActualExpenseViewModel, int>(
				this, 0, OpenPageOptions.IgnoreHash, card => card.ApplyTemplate(template.Id));
			page.ViewModel.EntitySaved += (_, _) => Reload();
		});
	}
}
