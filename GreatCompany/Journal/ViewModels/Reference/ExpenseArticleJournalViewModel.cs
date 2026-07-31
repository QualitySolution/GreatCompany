using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Dialog;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class ExpenseArticleJournalViewModel : ReferenceJournalViewModelBase<ExpenseArticle> {
	readonly Repository repo;

	public ExpenseArticleJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Статьи расхода";
		Reload();
	}

	protected override IEnumerable<ExpenseArticle> Load(string search) => repo.ExpenseArticles(search);
	protected override void Create() => OpenCard<ExpenseArticleViewModel>(0);
	protected override void Edit(ExpenseArticle row) => OpenCard<ExpenseArticleViewModel>(row.Id);
	protected override void Delete(ExpenseArticle row) { if(repo.Delete<ExpenseArticle>(row.Id)) Reload(); else NotifyDeleteBlocked(); }
}
