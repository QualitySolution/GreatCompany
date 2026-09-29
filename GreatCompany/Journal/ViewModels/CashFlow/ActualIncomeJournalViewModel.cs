using GreatCompany.Data.Models;
using GreatCompany.ViewModels.CashFlow;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class ActualIncomeJournalViewModel : IncomeJournalViewModelBase<ActualIncome, ActualIncomeViewModel> {
	public ActualIncomeJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IProjection PlannedIdProjection(IQueryOver<ActualIncome, ActualIncome> query) {
		PlannedIncome plannedAlias = null!;
		query.JoinAlias(x => x.PlannedIncome, () => plannedAlias, JoinType.LeftOuterJoin);
		return Projections.Property(() => plannedAlias.Id);
	}
}
