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

public class ProjectJournalViewModel : EntityJournalViewModelBase<Project, ProjectViewModel, ProjectJournalNode> {
	public ProjectJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		Func<IJournalViewModel, ProjectFilterViewModel> filterFactory,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
		JournalFilter = Filter = filterFactory(this);
	}

	public ProjectFilterViewModel Filter { get; }

	protected override IQueryOver<Project> ItemsQuery(IUnitOfWork uow) {
		Project projectAlias = null!;
		Division divisionAlias = null!;
		ProjectJournalNode resultAlias = null!;

		var query = uow.Session.QueryOver(() => projectAlias)
			.JoinAlias(() => projectAlias.Division, () => divisionAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => projectAlias.Id,
				() => projectAlias.Name,
				() => divisionAlias.Name));

		if(!Filter.ShowArchived)
			query.Where(() => !projectAlias.IsArchived);

		return query
			.SelectList(list => list
				.Select(() => projectAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => projectAlias.Name).WithAlias(() => resultAlias.Name)
				.Select(() => divisionAlias.Name).WithAlias(() => resultAlias.DivisionName)
				.Select(() => projectAlias.IsArchived).WithAlias(() => resultAlias.IsArchived))
			.OrderBy(() => projectAlias.Name).Asc
			.TransformUsing(Transformers.AliasToBean<ProjectJournalNode>());
	}
}

public class ProjectJournalNode {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string? DivisionName { get; set; }
	public bool IsArchived { get; set; }
	public string RowColor => IsArchived ? "gray" : "black";
}
