using GreatCompany.Data.Models;
using GreatCompany.Journal.ViewModels.Templates;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;
using QS.ViewModels.Dialog;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public abstract class ExpenseJournalViewModelBase<TEntity, TEntityViewModel>
	: CashFlowJournalViewModelBase<TEntity, TEntityViewModel, ExpenseJournalNode, PaymentTemplateJournalViewModel>
	where TEntity : ExpenseOperation
	where TEntityViewModel : DialogViewModelBase {

	protected ExpenseJournalViewModelBase(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<TEntity> ItemsQuery(IUnitOfWork uow) {
		TEntity expenseAlias = null!;
		Account accountAlias = null!;
		Division divisionAlias = null!;
		Project projectAlias = null!;
		ExpenseArticle articleAlias = null!;
		ExpenseJournalNode resultAlias = null!;

		var query = uow.Session.QueryOver(() => expenseAlias)
			.JoinAlias(() => expenseAlias.Account, () => accountAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => expenseAlias.Division, () => divisionAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => expenseAlias.Project, () => projectAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => expenseAlias.ExpenseArticle, () => articleAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => expenseAlias.Id,
				() => expenseAlias.Purpose,
				() => projectAlias.Name));

		var plannedId = PlannedIdProjection(query);

		return query.SelectList(list => {
				list.Select(() => expenseAlias.Id).WithAlias(() => resultAlias.Id)
					.Select(() => expenseAlias.Date).WithAlias(() => resultAlias.Date)
					.Select(() => expenseAlias.Purpose).WithAlias(() => resultAlias.Purpose)
					.Select(() => expenseAlias.Amount).WithAlias(() => resultAlias.Amount)
					.Select(() => expenseAlias.VatAmount).WithAlias(() => resultAlias.VatAmount)
					.Select(() => accountAlias.Name).WithAlias(() => resultAlias.AccountName)
					.Select(() => divisionAlias.Name).WithAlias(() => resultAlias.DivisionName)
					.Select(() => projectAlias.Name).WithAlias(() => resultAlias.ProjectName)
					.Select(() => articleAlias.Name).WithAlias(() => resultAlias.ArticleName);
				if(plannedId != null)
					list.Select(plannedId).WithAlias(() => resultAlias.PlannedId);
				return list;
			})
			.OrderBy(() => expenseAlias.Date).Desc
			.TransformUsing(Transformers.AliasToBean<ExpenseJournalNode>());
	}

	/// <summary>
	/// ссылка на план есть только у факта, поэтому джойн к ней добавляет наследник и возвращает проекцию номера плана.
	/// у плана ссылки нет
	/// </summary>
	protected virtual IProjection? PlannedIdProjection(IQueryOver<TEntity, TEntity> query) => null;
}

public class ExpenseJournalNode {
	public int Id { get; set; }
	public DateTime Date { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string? AccountName { get; set; }
	public string? DivisionName { get; set; }
	public string? ProjectName { get; set; }
	public string? ArticleName { get; set; }
	public int? PlannedId { get; set; }

	public string Title => $"{Purpose} от {Date:dd.MM.yyyy}";
}
