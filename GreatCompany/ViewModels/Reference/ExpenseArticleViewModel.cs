using GreatCompany.Data;
using GreatCompany.Data.Models;
using QS.Dialog;
using QS.DomainModel.Entity;
using QS.Navigation;
using QS.Validation;

namespace GreatCompany.ViewModels.Reference;

public class ExpenseArticleViewModel : FormViewModelBase {
	readonly Repository repo;

	public ExpenseArticleViewModel(int id, Repository repo, INavigationManager navigation, IValidator validator, IInteractiveMessage interactive)
		: base(navigation, validator, interactive) {
		this.repo = repo;
		Entity = id == 0 ? new ExpenseArticle() : repo.Get<ExpenseArticle>(id) ?? new ExpenseArticle();
		Title = Entity.Id == 0 ? "Новая статья расхода" : Entity.Name;

		TrackChanges(Entity);
	}

	public ExpenseArticle Entity { get; }

	protected override IDomainObject? SaveEntity() {
		if(!Validate(Entity))
			return null;
		repo.Save(Entity);
		return Entity;
	}
}
