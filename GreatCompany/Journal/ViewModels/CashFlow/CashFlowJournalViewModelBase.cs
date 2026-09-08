using QS.DomainModel.Entity;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Journal;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Domain;
using QS.Project.Journal;
using QS.Project.Services;
using QS.ViewModels.Dialog;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public abstract class CashFlowJournalViewModelBase<TEntity, TEntityViewModel, TNode, TTemplateJournal>
	: EntityJournalViewModelBase<TEntity, TEntityViewModel, TNode>
	where TEntity : class, IDomainObject
	where TEntityViewModel : DialogViewModelBase
	where TNode : class
	where TTemplateJournal : class, IJournalViewModel {

	protected CashFlowJournalViewModelBase(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override void CreateNodeActions() {
		base.CreateNodeActions();
		ButtonActionsViewModel.AddAction("Создать по шаблону", selected => SelectTemplate());
	}

	private void SelectTemplate() {
		var page = NavigationManager.OpenViewModel<TTemplateJournal>(this, OpenPageOptions.AsSlave);
		if(page == null)
			return;

		page.ViewModel.SelectionMode = JournalSelectionMode.Single;
		page.ViewModel.OnSelectResult -= TemplateSelected;
		page.ViewModel.OnSelectResult += TemplateSelected;
	}

	private void TemplateSelected(object? sender, JournalSelectedEventArgs e) =>
		NavigationManager.OpenViewModel<TEntityViewModel, IEntityUoWBuilder, int>(
			this, EntityUoWBuilder.ForCreate(), e.SelectedObjects[0].GetId(), OpenPageOptions.IgnoreHash);
}
