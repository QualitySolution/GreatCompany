using GreatCompany.Data.Models;
using GreatCompany.ViewModels.CashFlow;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using QS.DomainModel.NotifyChange;
using QS.DomainModel.UoW;
using QS.Journal;
using QS.Navigation;
using QS.Permissions;
using QS.Project.Services;

namespace GreatCompany.Journal.ViewModels.CashFlow;

public class ActualTransferJournalViewModel : EntityJournalViewModelBase<ActualTransfer, ActualTransferViewModel, ActualTransferJournalNode> {
	public ActualTransferJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<ActualTransfer> ItemsQuery(IUnitOfWork uow) {
		ActualTransfer transferAlias = null!;
		Account fromAlias = null!;
		Account toAlias = null!;
		ActualTransferJournalNode resultAlias = null!;

		return uow.Session.QueryOver(() => transferAlias)
			.JoinAlias(() => transferAlias.FromAccount, () => fromAlias, JoinType.LeftOuterJoin)
			.JoinAlias(() => transferAlias.ToAccount, () => toAlias, JoinType.LeftOuterJoin)
			.Where(GetSearchCriterion(
				() => transferAlias.Id,
				() => transferAlias.Purpose,
				() => fromAlias.Name,
				() => toAlias.Name))
			.SelectList(list => list
				.Select(() => transferAlias.Id).WithAlias(() => resultAlias.Id)
				.Select(() => transferAlias.Date).WithAlias(() => resultAlias.Date)
				.Select(() => transferAlias.Purpose).WithAlias(() => resultAlias.Purpose)
				.Select(() => transferAlias.Cost).WithAlias(() => resultAlias.Cost)
				.Select(() => transferAlias.IsLoan).WithAlias(() => resultAlias.IsLoan)
				.Select(() => fromAlias.Name).WithAlias(() => resultAlias.FromAccountName)
				.Select(() => toAlias.Name).WithAlias(() => resultAlias.ToAccountName))
			.OrderBy(() => transferAlias.Date).Desc
			.TransformUsing(Transformers.AliasToBean<ActualTransferJournalNode>());
	}
}

public class ActualTransferJournalNode {
	public int Id { get; set; }
	public DateTime Date { get; set; }
	public string Purpose { get; set; } = "";
	public decimal Cost { get; set; }
	public bool IsLoan { get; set; }
	public string? FromAccountName { get; set; }
	public string? ToAccountName { get; set; }

	public string Title => $"{Purpose} от {Date:dd.MM.yyyy}";
}
