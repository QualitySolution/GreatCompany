using GreatCompany.Data;
using GreatCompany.Data.Models;
using GreatCompany.ViewModels.Reference;
using QS.Dialog;
using QS.Navigation;

namespace GreatCompany.Journal.ViewModels.Reference;

public class IncomeArticleJournalViewModel : ReferenceJournalViewModelBase<IncomeArticle> {
	readonly Repository repo;

	public IncomeArticleJournalViewModel(Repository repo, INavigationManager navigation, IInteractiveMessage interactive) : base(navigation, interactive) {
		this.repo = repo;
		Title = "Статьи дохода";
		Reload();
	}

	protected override IEnumerable<IncomeArticle> Load(string search) => repo.IncomeArticles(search);
	protected override void Create() => OpenCard<IncomeArticleViewModel>(0);
	protected override void Edit(IncomeArticle row) => OpenCard<IncomeArticleViewModel>(row.Id);
	protected override void Delete(IncomeArticle row) { if(repo.Delete<IncomeArticle>(row.Id)) Reload(); else NotifyDeleteBlocked(); }
}
