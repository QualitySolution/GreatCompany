using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Journal;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.Reference;

public class DivisionJournalViewModel : EntityJournalViewModelBase<Division, DivisionViewModel, DivisionJournalNode> {
	public DivisionJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<Division> ItemsQuery(IUnitOfWork uow) {
		Division divisionAlias = null!;
		Division parentAlias = null!;
		DivisionJournalNode resultAlias = null!;

		return uow.Session.QueryOver(() => divisionAlias)
			.JoinAlias(() => divisionAlias.ParentDivision, () => parentAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => divisionAlias.Id,
				() => divisionAlias.Name,
				() => parentAlias.Name))
			.SelectList(list => list
				.Select(() => divisionAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => divisionAlias.Name).WithAlias(() => resultAlias.Name)
				.Select(() => parentAlias.Name).WithAlias(() => resultAlias.ParentName))
			.OrderBy(() => divisionAlias.Name).Asc
			.TransformUsing(Transformers.AliasToBean<DivisionJournalNode>());
	}
}

public class DivisionJournalNode {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string? ParentName { get; set; }
}
