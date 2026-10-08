using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Templates;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Journal;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.Templates;

public class AccrualTemplateJournalViewModel : EntityJournalViewModelBase<AccrualTemplate, AccrualTemplateViewModel, AccrualTemplateJournalNode> {
	public AccrualTemplateJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		Func<IJournalViewModel, TemplateFilterViewModel> filterFactory,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
		JournalFilter = Filter = filterFactory(this);
	}

	public TemplateFilterViewModel Filter { get; }

	protected override IQueryOver<AccrualTemplate> ItemsQuery(IUnitOfWork uow) {
		AccrualTemplate templateAlias = null!;
		Account accountAlias = null!;
		Project projectAlias = null!;
		IncomeArticle articleAlias = null!;
		AccrualTemplateJournalNode resultAlias = null!;

		var query = uow.Session.QueryOver(() => templateAlias)
			.JoinAlias(() => templateAlias.Account, () => accountAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => templateAlias.Project, () => projectAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => templateAlias.IncomeArticle, () => articleAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => templateAlias.Id,
				() => templateAlias.Purpose,
				() => projectAlias.Name));

		if(!Filter.ShowDisabled)
			query.Where(() => !templateAlias.IsDisabled);

		return query
			.SelectList(list => list
				.Select(() => templateAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => templateAlias.Purpose).WithAlias(() => resultAlias.Purpose)
				.Select(() => templateAlias.Cost).WithAlias(() => resultAlias.Cost)
				.Select(() => templateAlias.Vat).WithAlias(() => resultAlias.Vat)
				.Select(() => accountAlias.Name).WithAlias(() => resultAlias.AccountName)
				.Select(() => projectAlias.Name).WithAlias(() => resultAlias.ProjectName)
				.Select(() => articleAlias.Name).WithAlias(() => resultAlias.ArticleName)
				.Select(() => templateAlias.IsDisabled).WithAlias(() => resultAlias.IsDisabled))
			.OrderBy(() => templateAlias.Purpose).Asc
			.TransformUsing(Transformers.AliasToBean<AccrualTemplateJournalNode>());
	}
}

public class AccrualTemplateJournalNode {
	public int Id { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Cost { get; set; }
	public decimal Vat { get; set; }
	public string? AccountName { get; set; }
	public string? ProjectName { get; set; }
	public string? ArticleName { get; set; }
	public bool IsDisabled { get; set; }
	public string RowColor => IsDisabled ? "gray" : "black";
}
