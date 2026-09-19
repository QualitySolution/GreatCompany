using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using NHibernate;
using NHibernate.Transform;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Journal;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.Reference;

public class ExpenseArticleJournalViewModel : EntityJournalViewModelBase<ExpenseArticle, ExpenseArticleViewModel, ExpenseArticleJournalNode> {
	public ExpenseArticleJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<ExpenseArticle> ItemsQuery(IUnitOfWork uow) {
		ExpenseArticleJournalNode resultAlias = null!;

		return uow.Session.QueryOver<ExpenseArticle>()
			.Where(GetSearchCriterion<ExpenseArticle>(
				x => x.Id,
				x => x.Name))
			.SelectList(list => list
				.Select(x => x.Id).WithAlias(() => resultAlias.Id)
				.Select(x => x.Name).WithAlias(() => resultAlias.Name))
			.OrderBy(x => x.Name).Asc
			.TransformUsing(Transformers.AliasToBean<ExpenseArticleJournalNode>());
	}
}

public class ExpenseArticleJournalNode {
	public int Id { get; set; }
	public string Name { get; set; } = "";
}
