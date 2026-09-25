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

public class PaymentTemplateJournalViewModel : EntityJournalViewModelBase<PaymentTemplate, PaymentTemplateViewModel, PaymentTemplateJournalNode> {
	public PaymentTemplateJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<PaymentTemplate> ItemsQuery(IUnitOfWork uow) {
		PaymentTemplate templateAlias = null!;
		Account accountAlias = null!;
		Division divisionAlias = null!;
		Project projectAlias = null!;
		ExpenseArticle articleAlias = null!;
		PaymentTemplateJournalNode resultAlias = null!;

		return uow.Session.QueryOver(() => templateAlias)
			.JoinAlias(() => templateAlias.Account, () => accountAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => templateAlias.Division, () => divisionAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => templateAlias.Project, () => projectAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => templateAlias.ExpenseArticle, () => articleAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => templateAlias.Id,
				() => templateAlias.Purpose,
				() => projectAlias.Name))
			.SelectList(list => list
				.Select(() => templateAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => templateAlias.Purpose).WithAlias(() => resultAlias.Purpose)
				.Select(() => templateAlias.Amount).WithAlias(() => resultAlias.Amount)
				.Select(() => templateAlias.VatAmount).WithAlias(() => resultAlias.VatAmount)
				.Select(() => accountAlias.Name).WithAlias(() => resultAlias.AccountName)
				.Select(() => divisionAlias.Name).WithAlias(() => resultAlias.DivisionName)
				.Select(() => projectAlias.Name).WithAlias(() => resultAlias.ProjectName)
				.Select(() => articleAlias.Name).WithAlias(() => resultAlias.ArticleName))
			.OrderBy(() => templateAlias.Purpose).Asc
			.TransformUsing(Transformers.AliasToBean<PaymentTemplateJournalNode>());
	}
}

public class PaymentTemplateJournalNode {
	public int Id { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string? AccountName { get; set; }
	public string? DivisionName { get; set; }
	public string? ProjectName { get; set; }
	public string? ArticleName { get; set; }
}
