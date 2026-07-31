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

public class ActualIncomeJournalViewModel : JournalViewModelBase<ActualIncomeRow> {
	readonly Repository repo;

	public ActualIncomeJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Факт - приход";
		CreateFromTemplateCommand = ReactiveCommand.Create(CreateFromTemplate);
		Reload();
	}

	public ReactiveCommand<Unit, Unit> CreateFromTemplateCommand { get; }

	protected override IEnumerable<ActualIncomeRow> Load(string search) => repo.ActualIncomes(search);
	protected override void Create() => OpenCard<ActualIncomeViewModel>(0);
	protected override void Edit(ActualIncomeRow row) => OpenCard<ActualIncomeViewModel>(row.Id);
	protected override void Delete(ActualIncomeRow row) { if(repo.Delete<ActualIncome>(row.Id)) Reload(); else NotifyDeleteBlocked(); }

	void CreateFromTemplate() {
		NavigationManager.OpenReferenceSelect<AccrualTemplateJournalViewModel>(this, template => {
			var page = NavigationManager.OpenViewModel<ActualIncomeViewModel, int>(
				this, 0, OpenPageOptions.IgnoreHash, card => card.ApplyTemplate(template.Id));
			page.ViewModel.EntitySaved += (_, _) => Reload();
		});
	}
}
