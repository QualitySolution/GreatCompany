using GreatCompany.Data.Models;
using GreatCompany.ViewModels.CashFlow;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class PlannedIncomeJournalViewModel : IncomeJournalViewModelBase<PlannedIncome, PlannedIncomeViewModel> {
	public PlannedIncomeJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}
}
