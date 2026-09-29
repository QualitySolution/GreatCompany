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
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<Project> ItemsQuery(IUnitOfWork uow) {
		Project projectAlias = null!;
		Division divisionAlias = null!;
		ProjectJournalNode resultAlias = null!;

		return uow.Session.QueryOver(() => projectAlias)
			.JoinAlias(() => projectAlias.Division, () => divisionAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => projectAlias.Id,
				() => projectAlias.Name,
				() => divisionAlias.Name))
			.SelectList(list => list
				.Select(() => projectAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => projectAlias.Name).WithAlias(() => resultAlias.Name)
				.Select(() => divisionAlias.Name).WithAlias(() => resultAlias.DivisionName))
			.OrderBy(() => projectAlias.Name).Asc
			.TransformUsing(Transformers.AliasToBean<ProjectJournalNode>());
	}
}

public class ProjectJournalNode {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public string? DivisionName { get; set; }
}
