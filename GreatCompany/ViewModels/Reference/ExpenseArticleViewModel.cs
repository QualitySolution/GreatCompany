using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Navigation;

namespace GreatCompany.ViewModels.Reference;

public class ExpenseArticleViewModel : FormViewModelBase {
	readonly Repository repo;

	public ExpenseArticleViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new ExpenseArticle() : repo.Get<ExpenseArticle>(id) ?? new ExpenseArticle();
		Title = id == 0 ? "Новая статья расхода" : Entity.Name;
	}

	public ExpenseArticle Entity { get; }

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Name))
			return false;
		repo.Save(Entity);
		return true;
	}
}
