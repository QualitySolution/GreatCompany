using Gamma.Utilities;
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

public class AccountJournalViewModel : EntityJournalViewModelBase<Account, AccountViewModel, AccountJournalNode> {
	public AccountJournalViewModel(
		IUnitOfWorkFactory unitOfWorkFactory,
		INavigationManager navigationManager,
		IEntityChangeWatcher changeWatcher,
		IDeleteEntityService? deleteEntityService = null,
		ICurrentPermissionService? currentPermissionService = null)
		: base(unitOfWorkFactory, navigationManager, changeWatcher, deleteEntityService, currentPermissionService) {
	}

	protected override IQueryOver<Account> ItemsQuery(IUnitOfWork uow) {
		AccountJournalNode resultAlias = null!;

		return uow.Session.QueryOver<Account>()
			.Where(GetSearchCriterion<Account>(
				x => x.Id,
				x => x.Name))
			.SelectList(list => list
				.Select(x => x.Id).WithAlias(() => resultAlias.Id)
				.Select(x => x.Name).WithAlias(() => resultAlias.Name)
				.Select(x => x.TaxRegime).WithAlias(() => resultAlias.TaxRegime))
			.OrderBy(x => x.Name).Asc
			.TransformUsing(Transformers.AliasToBean<AccountJournalNode>());
	}
}

public class AccountJournalNode {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public TaxRegime TaxRegime { get; set; }
	public string TaxRegimeTitle => TaxRegime.GetEnumTitle();
}
