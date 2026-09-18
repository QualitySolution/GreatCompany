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

public abstract class IncomeJournalViewModelBase<TEntity, TEntityViewModel>
	: CashFlowJournalViewModelBase<TEntity, TEntityViewModel, IncomeJournalNode, AccrualTemplateJournalViewModel>
	where TEntity : IncomeOperation
	where TEntityViewModel : DialogViewModelBase {

	protected IncomeJournalViewModelBase(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<TEntity> ItemsQuery(IUnitOfWork uow) {
		TEntity incomeAlias = null!;
		Account accountAlias = null!;
		Project projectAlias = null!;
		IncomeArticle articleAlias = null!;
		IncomeJournalNode resultAlias = null!;

		var query = uow.Session.QueryOver(() => incomeAlias)
			.JoinAlias(() => incomeAlias.Account, () => accountAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => incomeAlias.Project, () => projectAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => incomeAlias.IncomeArticle, () => articleAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => incomeAlias.Id,
				() => incomeAlias.Purpose,
				() => projectAlias.Name));

		var plannedId = PlannedIdProjection(query);

		return query.SelectList(list => {
				list.Select(() => incomeAlias.Id).WithAlias(() => resultAlias.Id)
					.Select(() => incomeAlias.Date).WithAlias(() => resultAlias.Date)
					.Select(() => incomeAlias.Purpose).WithAlias(() => resultAlias.Purpose)
					.Select(() => incomeAlias.Amount).WithAlias(() => resultAlias.Amount)
					.Select(() => incomeAlias.VatAmount).WithAlias(() => resultAlias.VatAmount)
					.Select(() => accountAlias.Name).WithAlias(() => resultAlias.AccountName)
					.Select(() => projectAlias.Name).WithAlias(() => resultAlias.ProjectName)
					.Select(() => articleAlias.Name).WithAlias(() => resultAlias.ArticleName);
				if(plannedId != null)
					list.Select(plannedId).WithAlias(() => resultAlias.PlannedId);
				return list;
			})
			.OrderBy(() => incomeAlias.Date).Desc
			.TransformUsing(Transformers.AliasToBean<IncomeJournalNode>());
	}

	/// <summary>
	/// Ссылка на план есть только у факта, поэтому джойн к ней добавляет наследник
	/// и возвращает проекцию номера плана
	/// У плана ссылки нет
	/// </summary>
	protected virtual IProjection? PlannedIdProjection(IQueryOver<TEntity, TEntity> query) => null;
}

public class IncomeJournalNode {
	public int Id { get; set; }
	public DateTime Date { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Amount { get; set; }
	public decimal VatAmount { get; set; }
	public string? AccountName { get; set; }
	public string? ProjectName { get; set; }
	public string? ArticleName { get; set; }
	public int? PlannedId { get; set; }

	public string Title => $"{Purpose} от {Date:dd.MM.yyyy}";
}
