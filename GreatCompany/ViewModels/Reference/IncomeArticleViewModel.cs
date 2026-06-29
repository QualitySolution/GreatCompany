using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Navigation;

namespace GreatCompany.ViewModels.Reference;

public class IncomeArticleViewModel : FormViewModelBase {
	readonly Repository repo;

	public IncomeArticleViewModel(int id, Repository repo, INavigationManager navigation) : base(navigation) {
		this.repo = repo;
		Entity = id == 0 ? new IncomeArticle() : repo.Get<IncomeArticle>(id) ?? new IncomeArticle();
		Title = id == 0 ? "Новая статья дохода" : Entity.Name;
	}

	public IncomeArticle Entity { get; }

	protected override bool Save() {
		if(string.IsNullOrWhiteSpace(Entity.Name))
			return false;
		repo.Save(Entity);
		return true;
	}
}
